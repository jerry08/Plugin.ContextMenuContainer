using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Plugin.ContextMenuContainer;

public class ContextMenuContainer : ContentView
{
    /// <summary>
    /// Call this in order to preserve our code during linking and allow namespace resolution in XAML
    /// </summary>
    public static void Init()
    {
        //maybe do something here later
    }

    public static readonly BindableProperty MenuItemsProperty = BindableProperty.Create(
        nameof(MenuItems),
        typeof(ContextMenuItems),
        typeof(VisualElement),
        defaultValueCreator: DefaulfMenuItemsCreator,
        propertyChanged: OnMenuItemsChanged
    );

    private static object DefaulfMenuItemsCreator(BindableObject bindableObject)
    {
        var menuItems = new ContextMenuItems();
        menuItems.CollectionChanged += (s, e) =>
        {
            if (e.OldItems is not null)
                foreach (ContextMenuItem item in e.OldItems)
                    item.RemoveBinding(ContextMenuItem.BindingContextProperty);

            if (e.NewItems is not null)
                foreach (ContextMenuItem item in e.NewItems)
                    BindableObject.SetInheritedBindingContext(item, bindableObject.BindingContext);
        };

        return menuItems;
    }

    private static void OnMenuItemsChanged(
        BindableObject bindableObject,
        object newValue,
        object oldValue
    )
    {
        if (oldValue is ContextMenuItems oldItems)
        {
            foreach (var item in oldItems)
                item.RemoveBinding(ContextMenuContainer.BindingContextProperty);

            //oldItems.CollectionChanged -= MenuItems_CollectionChanged;
        }

        if (newValue is ContextMenuItems newItems)
            foreach (var item in newItems)
                BindableObject.SetInheritedBindingContext(item, bindableObject.BindingContext);
    }

    //private static void MenuItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //{
    //    if (e.OldItems != null)
    //    {
    //        foreach (ContextMenuItem item in e.OldItems)
    //        {
    //            item.RemoveBinding(ContextMenuItem.BindingContextProperty);
    //        }
    //    }
    //    if (e.NewItems != null)
    //    {
    //        ((ContextMenuContainer)sender).SetBindingContextForItems((IList<ContextMenuItem>)e.NewItems);
    //    }
    //}

    public ContextMenuItems MenuItems
    {
        get => (ContextMenuItems)GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        SetBindingContextForItems(MenuItems);
    }

    private void SetBindingContextForItems(IList<ContextMenuItem> items)
    {
        for (var i = 0; i < items.Count; i++)
            SetBindingContextForItem(items[i]);
    }

    private void SetBindingContextForItem(ContextMenuItem item)
    {
        BindableObject.SetInheritedBindingContext(item, BindingContext);
    }

    public ContextMenuContainer()
    {
        //GestureRecognizers.Add();
    }

    public void Show()
    {
        OpenContextMenu?.Invoke();
    }

    internal OpenContextMenuDelegate? OpenContextMenu;

    public delegate void OpenContextMenuDelegate();
}
