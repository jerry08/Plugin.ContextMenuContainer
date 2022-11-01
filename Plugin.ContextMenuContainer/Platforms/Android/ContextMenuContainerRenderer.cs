using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Widget;
using DrawableWrapperX = AndroidX.AppCompat.Graphics.Drawable.DrawableWrapper;
using Java.Lang.Reflect;
using Path = System.IO.Path;
using AColor = Android.Graphics.Color;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;

using ViewRenderer = Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer;

namespace Plugin.ContextMenuContainer;

public class ContextMenuContainerRenderer : ViewRenderer
{
    private PopupMenu? contextMenu;

    public ContextMenuContainerRenderer(Context context) : base(context)
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Microsoft.Maui.Controls.View> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement is ContextMenuContainer old)
        {
            old.BindingContextChanged -= Element_BindingContextChanged;
            old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
            
            old.OpenContextMenu = null;
        }

        if (e.NewElement is ContextMenuContainer newElement)
        {
            newElement.BindingContextChanged += Element_BindingContextChanged;
            newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;

            newElement.OpenContextMenu = OpenContextMenu;
        }
    }

    private void ConstructNativeMenu()
    {
        var child = GetChildAt(0);
        if (child is null)
            return;
        contextMenu = new PopupMenu(Context, child);
        contextMenu.MenuItemClick += ContextMenu_MenuItemClick;
        var field = contextMenu.Class.GetDeclaredField("mPopup");
        field.Accessible = true;
        var menuPopupHelper = field.Get(contextMenu);
        var setForceIcons = menuPopupHelper?.Class.GetDeclaredMethod("setForceShowIcon", Java.Lang.Boolean.Type!);
        setForceIcons?.Invoke(menuPopupHelper, true);
    }

    private void DeconstructNativeMenu()
    {
        if (contextMenu is null)
            return;
        contextMenu.MenuItemClick -= ContextMenu_MenuItemClick;
        contextMenu.Dispose();
        contextMenu = null;
    }

    private void AddMenuItem(ContextMenuItem item)
    {
        if (contextMenu is null)
            return;

        var title = new SpannableString(item.Text);
        if (item.IsDestructive)
            title.SetSpan(new ForegroundColorSpan(AColor.Red), 0, title.Length(), 0);

        var contextAction = contextMenu.Menu.Add(title);
        if (contextAction is null)
        {
            Logger.Error("We couldn't create IMenuItem with title {0}", item.Text);
            return;
        }

        contextAction.SetEnabled(item.IsEnabled);

        if (item.Icon is not null)
        {
            var name = Path.GetFileNameWithoutExtension(item.Icon.File);
            var id = Context.GetDrawableId(name);
            if (id != 0)
            {
                var drawable = (int)Build.VERSION.SdkInt >= 21 ?
                     Context?.GetDrawable(id) :
                     Context?.GetDrawable(name);
                if (drawable is not null)
                {
                    var wrapper = new DrawableWrapperX(drawable);
                    if (item.IsDestructive)
                        wrapper.SetTint(AColor.Red);

                    contextAction.SetIcon(wrapper);
                }
            }
        }
    }

    private void FillMenuItems()
    {
        if (Element is ContextMenuContainer element)
        {
            if (element.MenuItems.Count > 0)
            {
                foreach (var item in element.MenuItems)
                {
                    AddMenuItem(item);
                }
            }
        }
    }

    private void RefillMenuItems()
    {
        if (contextMenu == null)
            return;
        contextMenu.Dismiss();
        contextMenu.Menu.Clear();
        FillMenuItems();
    }

    private PopupMenu? GetContextMenu()
    {
        if (contextMenu != null && Element is ContextMenuContainer element)
        {
            if (element.MenuItems.Count != contextMenu.Menu.Size())
            {
                DeconstructNativeMenu();
            }
            else
            {
                for (int i = 0; i < contextMenu.Menu.Size(); i++)
                {
                    if (!element.MenuItems[i].Text.Equals(contextMenu.Menu.GetItem(i)?.TitleFormatted?.ToString()))
                    {
                        DeconstructNativeMenu();
                        break;
                    }
                }
            }
        }
        return contextMenu;
    }

    private void MenuItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        RefillMenuItems();
    }

    private void Element_BindingContextChanged(object? sender, EventArgs e)
    {
        RefillMenuItems();
    }

    private void ContextMenu_MenuItemClick(object? sender, PopupMenu.MenuItemClickEventArgs e)
    {
        var item = ((ContextMenuContainer)Element!).MenuItems.FirstOrDefault(x => x.Text == e.Item.TitleFormatted?.ToString());
        item?.OnItemTapped();
    }

    private bool enabled => Element is ContextMenuContainer element && element.MenuItems.Count > 0;
    private MyTimer timer = default!;
    private bool timerFired = false;

    public override bool DispatchTouchEvent(MotionEvent? e)
    {
        bool result;
        Logger.Debug("ContextMEnuContainer DispatchTouchEvent fired {0}", e!.Action);
        if (enabled && e.Action == MotionEventActions.Down)
        {
            //You can change the timespan of the long press
            timerFired = false;
            timer = new MyTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                timerFired = true;
                OpenContextMenu();
            });
            timer.Start();
        }

        if (timerFired)
        {
            result = true;
        }
        else if (e.Action == MotionEventActions.Up || e.Action == MotionEventActions.Cancel)
        {
            timer?.Stop();
            result = base.DispatchTouchEvent(e);
        }
        else
        {
            result =  base.DispatchTouchEvent(e);
            
            if(!result && enabled)
                result = true;
        }

        return result;
    }

    private void OpenContextMenu()
    {
        if (GetContextMenu() is null)
        {
            ConstructNativeMenu();
            FillMenuItems();
        }

        contextMenu?.Show();
    }

    private class MyTimer
    {
        private readonly TimeSpan timespan;
        private readonly Action callback;

        private CancellationTokenSource cancellation;

        public MyTimer(TimeSpan timespan, Action callback)
        {
            this.timespan = timespan;
            this.callback = callback;
            this.cancellation = new CancellationTokenSource();
        }

        public void Start()
        {
            var cts = this.cancellation; // safe copy
            Device.StartTimer(this.timespan,
                () =>
                {
                    if (cts.IsCancellationRequested)
                        return false;
                    
                    this.callback.Invoke();

                    return false; // or true for periodic behavior
                });
        }

        public void Stop()
        {
            Interlocked.Exchange(ref this.cancellation, new CancellationTokenSource()).Cancel();
        }
    }
}