﻿using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using UIKit;

namespace Plugin.ContextMenuContainer;

internal class ContextMenuDelegate : UIContextMenuInteractionDelegate
{
    private readonly INSCopying? _identifier;
    private readonly Func<UIViewController>? _preview;
    private readonly ContextMenuItems _menuItems;
    private readonly Func<UIUserInterfaceStyle> _getCurrentTheme;
    private UIMenu? _nativeMenu;

    public ContextMenuDelegate(
        ContextMenuItems items,
        Func<UIUserInterfaceStyle> getCurrentTheme,
        INSCopying? identifier = null,
        Func<UIViewController>? preview = null
    )
    {
        _menuItems = items ?? throw new ArgumentNullException(nameof(items));
        _identifier = identifier;
        _preview = preview;
        _getCurrentTheme = getCurrentTheme;
    }

    public UIMenu? GetMenu()
    {
        ConstructMenuFromItems([]);
        return _nativeMenu;
    }

    public override UIContextMenuConfiguration GetConfigurationForMenu(
        UIContextMenuInteraction interaction,
        CGPoint location
    ) =>
        UIContextMenuConfiguration.Create(
            _identifier,
            _preview != null ? PreviewDelegate! : null,
            ConstructMenuFromItems
        );

    private IEnumerable<UIMenuElement> ToNativeActions(
        IEnumerable<ContextMenuItem> sharedDefinitions
    )
    {
        var iconColor =
            _getCurrentTheme() == UIUserInterfaceStyle.Dark ? UIColor.White : UIColor.Black;
        foreach (var item in sharedDefinitions)
        {
            if (!string.IsNullOrEmpty(item.Text))
            {
                UIImage? nativeImage = null;
                if (item.Icon != null && !string.IsNullOrWhiteSpace(item.Icon.File))
                {
                    nativeImage = new UIImage(item.Icon.File);
                    nativeImage = nativeImage.ImageWithRenderingMode(
                        UIImageRenderingMode.AlwaysTemplate
                    );
                    nativeImage.ApplyTintColor(item.IsDestructive ? UIColor.Red : iconColor);
                }

                var nativeItem = UIAction.Create(item.Text, nativeImage, item.Text, ActionDelegate);
                if (!item.IsEnabled)
                {
                    nativeItem.Attributes |= UIMenuElementAttributes.Disabled;
                }

                if (item.IsDestructive)
                {
                    nativeItem.Attributes |= UIMenuElementAttributes.Destructive;
                }

                yield return nativeItem;
            }
            else
            {
                Logger.Error("ContextMenuItem text should not be empty!");
            }
        }
    }

    private void ActionDelegate(UIAction action) => _menuItems[action.Identifier].OnItemTapped();

    private UIMenu ConstructMenuFromItems(UIMenuElement[] suggestedActions)
    {
        _nativeMenu =
            _nativeMenu == null
                ? UIMenu.Create(ToNativeActions(_menuItems).ToArray())
                : _nativeMenu.GetMenuByReplacingChildren(ToNativeActions(_menuItems).ToArray());
        return _nativeMenu;
    }

    public UIViewController? PreviewDelegate() => _preview?.Invoke();
}
