# ContextMenuContainer
[![Version](https://img.shields.io/nuget/v/Plugin.ContextMenuContainer.svg)](https://nuget.org/packages/Plugin.ContextMenuContainer)
[![Downloads](https://img.shields.io/nuget/dt/Plugin.ContextMenuContainer.svg)](https://nuget.org/packages/Plugin.ContextMenuContainer)

Maui plugin to add native context menu to any view. Supports all .NET MAUI platforms.

## Usage
1. Add the `.ConfigureContextMenuContainer()` in your `MauiProgram.cs` as shown below:

```C#
using Plugin.ContextMenuContainer;
```

```C#
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkit()
    .ConfigureContextMenuContainer();
```

2. Add namespace to your XAML file
    `xmlns:cm="https://github.com/jerry08/Plugin.ContextMenuContainer"`

3. Wrap your view with `ContextMenuContainer`, define your context actions inline or bind from your ViewModel
```
//Inline
<cm:ContextMenuContainer x:Name="ActionsInline">
    <cm:ContextMenuContainer.MenuItems>
        <cm:ContextMenuItem
            Text="My action" 
            Command="{Binding MyCommand}" 
            CommandParameter="{Binding .}" />
        <cm:ContextMenuItem
            Text="My destructive action" 
            Command="{Binding MyDestructiveCommand}" 
            CommandParameter="{Binding .}" 
            IsDestructive="True" 
            Icon="{Binding DestructiveIconSource}"/>
    </cm:ContextMenuContainer.MenuItems>
    <cm:ContextMenuContainer.Content>
        <Label Text="Hold me!"/>
    </cm:ContextMenuContainer.Content>
</cm:ContextMenuContainer>
//From binding
<cm:ContextMenuContainer
    x:Name="ContextActionsWithBinding" 
    MenuItems="{Binding ImageContextItems}">
    <cm:ContextMenuContainer.Content>
        <Frame>
            <Image Source="{Binding IconSource}"/>
        </Frame>
    </cm:ContextMenuContainer.Content>
</cm:ContextMenuContainer>
```
