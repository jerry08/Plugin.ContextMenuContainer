using System;
using System.Linq;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using WColors = Microsoft.UI.Colors;
using WBinding = Microsoft.UI.Xaml.Data.Binding;
using Microsoft.Maui.Controls.Platform;
using MenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using Style = Microsoft.UI.Xaml.Style;
using Setter = Microsoft.UI.Xaml.Setter;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;

//[assembly: ExportRenderer(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer))]
namespace Plugin.ContextMenuContainer;

//[Preserve(AllMembers = true)]
public class ContextMenuContainerRenderer : Microsoft.Maui.Controls.Handlers.Compatibility.ViewRenderer<ContextMenuContainer, ContentControl>
{
    private FrameworkElement? content;

    public ContextMenuContainerRenderer()
    {
        AutoPackage = false;
    }
    protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
    {
        base.OnElementChanged(e);
        if (e.OldElement is not null)
        {
            //unsubscribe from events here
        }

        if (e.NewElement is null)
        {
            return;
        }

        if (Control is null)
        {
            SetNativeControl(new ContentControl());
        }

        Pack();
    }

    private void Pack()
    {
        if (Element?.Content is null)
            return;

        IVisualElementRenderer renderer = Element.Content.GetOrCreateRenderer();
        content = renderer.ContainerElement;
        content.PointerReleased += Content_PointerReleased;
        //content.Holding += FrameworkElement_Holding;
        Control!.Content = content;
    }

    private void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(content);
        if (point.Properties.PointerUpdateKind != PointerUpdateKind.RightButtonReleased)
            return;

        try
        {
            if (Element!.HasMenuOptions())
                OpenContextMenu();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private MenuFlyout? GetContextMenu()
    {
        if (FlyoutBase.GetAttachedFlyout(content) is MenuFlyout flyout)
        {
            var actions = Element!.MenuItems;
            if (flyout.Items.Count != actions.Count)
                return null;

            for (int i = 0; i < flyout.Items.Count; i++)
                if (flyout.Items[i].DataContext != actions[i])
                    return null;

            return flyout;
        }

        return null;
    }

    private void OpenContextMenu()
    {
        if (GetContextMenu() is null)
        {
            var flyout = new MenuFlyout();
            SetupMenuItems(flyout);

            Element!.MenuItems.CollectionChanged += MenuItems_CollectionChanged;

            FlyoutBase.SetAttachedFlyout(content, flyout);
        }

        FlyoutBase.ShowAttachedFlyout(content);
    }

    private void MenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var menu = GetContextMenu();
        if (menu != null)
        {
            menu.Items.Clear();
            SetupMenuItems(menu);
        }
    }

    private void SetupMenuItems(MenuFlyout menu)
    {
        foreach (var item in Element!.MenuItems)
        {
            AddMenuItem(menu, item);
        }
    }

    private void AddMenuItem(MenuFlyout contextMenu, ContextMenuItem item)
    {
        var nativeItem = new MenuFlyoutItem();
        nativeItem.SetBinding(MenuFlyoutItem.TextProperty, new WBinding()
        {
            Path = new PropertyPath(nameof(ContextMenuItem.Text)),
        });

        //nativeItem.SetBinding(MenuFlyoutItem.CommandProperty, new WBinding()
        //{
        //    Path = new PropertyPath(nameof(ContextMenuItem.Command)),
        //});

        //nativeItem.SetBinding(MenuFlyoutItem.CommandParameterProperty, new WBinding()
        //{
        //    Path = new PropertyPath(nameof(ContextMenuItem.CommandParameter)),
        //});

        nativeItem.SetBinding(MenuFlyoutItem.IconProperty, new WBinding()
        {
            Path = new PropertyPath(nameof(ContextMenuItem.Icon)),
            Converter = ImageConverter,
        });

        nativeItem.SetBinding(MenuFlyoutItem.StyleProperty, new WBinding()
        {
            Path = new PropertyPath(nameof(ContextMenuItem.IsDestructive)),
            Converter = BoolToStytleConverter,
        });

        nativeItem.SetBinding(MenuFlyoutItem.IsEnabledProperty, new WBinding()
        {
            Path = new PropertyPath(nameof(ContextMenuItem.IsEnabled)),
        });

        nativeItem.Click += NativeItem_Click;
        nativeItem.DataContext = item;
        contextMenu.Items.Add(nativeItem);
    }

    private void NativeItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuFlyoutItem item)
        {
            Logger.Error("Couldn't cast to MenuFlyoutItem");
            return;
        }

        if (item.DataContext is not ContextMenuItem context)
        {
            Logger.Error("Couldn't cast MenuFlyoutItem.DataContext to ContextMenuItem");
            return;
        }

        context.OnItemTapped();
    }

    private static Style DestructiveStyle { get; } = new Style()
    {
        TargetType = typeof(MenuFlyoutItem),
        Setters =
        {
            new Setter(MenuFlyoutItem.ForegroundProperty, new SolidColorBrush(WColors.Red)),
        }
    };

    private static Style NondDestructiveStyle { get; } = new Style()
    {
        TargetType = typeof(MenuFlyoutItem),
        Setters =
        {
            //new Setter(MenuFlyoutItem.ForegroundProperty, new SolidColorBrush(WColors.Red)),
        }
    };

    private static FileImageSourceToBitmapIconSourceConverter ImageConverter { get; } = new FileImageSourceToBitmapIconSourceConverter();

    private static GenericBoolConverter<Style> BoolToStytleConverter { get; } = new(DestructiveStyle, NondDestructiveStyle);
}