using System.Collections.ObjectModel;

namespace Plugin.ContextMenuContainer;

public static class Initialize
{
    public static void Init()
    {
        var c = new ContextMenuContainer();
        var i = new ContextMenuItem();
        var r = new ContextMenuContainerRenderer();
        c = null;
        i = null;
        r = null;
    }
}