﻿using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Plugin.ContextMenuContainer;

public class ContextMenuContainer : ContentView
{
    public static readonly BindableProperty MenuItemsProperty = BindableProperty.Create(
        nameof(MenuItems),
        typeof(ContextMenuItems),
        typeof(VisualElement),
        defaultValueCreator: DefaultMenuItemsCreator,
        propertyChanged: OnMenuItemsChanged
    );

    public ContextMenuItems? MenuItems
    {
        get => (ContextMenuItems?)GetValue(MenuItemsProperty);
        set => SetValue(MenuItemsProperty, value);
    }

    public static readonly BindableProperty ShowOnClickProperty = BindableProperty.Create(
        nameof(ShowOnClick),
        typeof(bool),
        typeof(ContextMenuContainer),
        true
    );

    public bool ShowOnClick
    {
        get => (bool)GetValue(ShowOnClickProperty);
        set => SetValue(ShowOnClickProperty, value);
    }

    /// <summary>
    /// Call this in order to preserve our code during linking and allow namespace resolution in XAML.
    /// </summary>
    public static void Init()
    {
        // maybe do something here later
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (MenuItems != null)
        {
            SetBindingContextForItems(MenuItems);
        }
    }

    private static object DefaultMenuItemsCreator(BindableObject bindableObject)
    {
        var menuItems = new ContextMenuItems();
        menuItems.CollectionChanged += (_, e) =>
        {
            if (e.OldItems != null)
            {
                foreach (ContextMenuItem item in e.OldItems)
                {
                    item.RemoveBinding(BindingContextProperty);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ContextMenuItem item in e.NewItems)
                {
                    SetInheritedBindingContext(item, bindableObject.BindingContext);
                }
            }
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
            {
                item.RemoveBinding(BindingContextProperty);
            }

            // oldItems.CollectionChanged -= MenuItems_CollectionChanged;
        }

        if (newValue is ContextMenuItems newItems)
        {
            foreach (var item in newItems)
            {
                SetInheritedBindingContext(item, bindableObject.BindingContext);
            }
        }
    }

    private void SetBindingContextForItems(IList<ContextMenuItem> items)
    {
        for (var i = 0; i < items.Count; i++)
        {
            SetBindingContextForItem(items[i]);
        }
    }

    private void SetBindingContextForItem(ContextMenuItem item) =>
        SetInheritedBindingContext(item, BindingContext);

    public void Show()
    {
        OpenContextMenu?.Invoke();
    }

    internal OpenContextMenuDelegate? OpenContextMenu { get; set; }

    public delegate void OpenContextMenuDelegate();
}
