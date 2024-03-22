using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Plugin.ContextMenuContainer;

internal sealed class ContextMenuContainerRenderer : ContentViewHandler
{
    private bool _wasSetOnce;

    private UIView Control => PlatformView;

    private UITraitCollection TraitCollection => UITraitCollection.CurrentTraitCollection;

    public override void SetVirtualView(IView view)
    {
        if (_wasSetOnce)
        {
            if (VirtualView is ContextMenuContainer old)
            {
                //old.OpenContextMenu = null;
                old.BindingContextChanged -= Element_BindingContextChanged;
                if (old.MenuItems != null)
                {
                    old.MenuItems.CollectionChanged -= MenuItems_CollectionChanged;
                }
            }
        }

        base.SetVirtualView(view);

        if (VirtualView is ContextMenuContainer newElement)
        {
            //newElement.OpenContextMenu = () => _contextMenuDelegate?.PreviewDelegate();
            //newElement.OpenContextMenu = () =>
            //{
            //    //var gs = Control.GestureRecognizers![2];
            //    //gs.TouchesBegan([], new UIEvent());
            //    //gs.TouchesEnded([], new UIEvent());
            //};

            newElement.BindingContextChanged += Element_BindingContextChanged;
            if (newElement.MenuItems != null)
            {
                newElement.MenuItems.CollectionChanged += MenuItems_CollectionChanged;
            }

            RefillMenuItems();

            if (newElement.ShowOnClick)
            {
                var buttonView = newElement
                    .Content.GetVisualTreeDescendants()
                    .OfType<IView>()
                    .FirstOrDefault(x => x is Button or ImageButton);
                if (buttonView is not null)
                {
                    var el = buttonView.ToPlatform(buttonView.Handler!.MauiContext!);
                    if (el is UIButton button)
                    {
                        button.ShowsMenuAsPrimaryAction = true;
                        button.Menu = _contextMenuDelegate?.GetMenu();
                    }
                }
            }

            //if (newElement.Content is ImageButton b)
            //{
            //    var list = newElement.Content.GetVisualTreeDescendants().ToList();
            //
            //    var el = b.ToPlatform(b.Handler!.MauiContext!);
            //    if (el is UIButton button)
            //    {
            //        button.ShowsMenuAsPrimaryAction = true;
            //        button.Menu = _contextMenuDelegate?.GetMenu();
            //    }
            //}

            _wasSetOnce = true;
        }
    }

    private void RefillMenuItems()
    {
        if (VirtualView is ContextMenuContainer container)
        {
            ConstructInteraction(container);
        }
    }

    private void MenuItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        RefillMenuItems();

    private void Element_BindingContextChanged(object? sender, EventArgs e) => RefillMenuItems();

#pragma warning disable SA1201
    private ContextMenuDelegate? _contextMenuDelegate;
#pragma warning restore SA1201
    private UIContextMenuInteraction? _contextMenu;

    private void DeconstructInteraction()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Control != null && _contextMenu != null)
        {
            Control.RemoveInteraction(_contextMenu);
        }
    }

    private void ConstructInteraction(ContextMenuContainer container)
    {
        DeconstructInteraction();
        if (container.MenuItems?.Count > 0)
        {
            _contextMenuDelegate = new ContextMenuDelegate(
                container.MenuItems,
                () => TraitCollection.UserInterfaceStyle
            );
            _contextMenu = new UIContextMenuInteraction(_contextMenuDelegate);
            Control.AddInteraction(_contextMenu);
        }
    }
}
