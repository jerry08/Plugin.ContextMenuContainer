namespace Plugin.ContextMenuContainer;

public static class ContextMenuContainerExtensions
{
    public static bool HasMenuOptions(this ContextMenuContainer container) =>
        container.MenuItems?.Count > 0;
}
