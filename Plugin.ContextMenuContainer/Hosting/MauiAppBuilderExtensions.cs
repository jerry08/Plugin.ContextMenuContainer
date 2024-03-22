using Microsoft.Maui.Hosting;

namespace Plugin.ContextMenuContainer;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureContextMenuContainer(this MauiAppBuilder builder)
    {
        return builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(ContextMenuContainer), typeof(ContextMenuContainerRenderer));
        });
    }
}
