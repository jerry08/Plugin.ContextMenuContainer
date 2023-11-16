using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using UIKit;

namespace Plugin.ContextMenuContainer;

public class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, UIView>
{
    ContextMenuDelegate? contextMenuDelegate;
    UIContextMenuInteraction? contextMenu;

    protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement is not null)
        {
            //do something with old element
        }

        if (e.NewElement is null || e.NewElement.Content is null)
            return;

        //var childRenderer = Platform.CreateRenderer(Element.Content);
        //SetNativeControl(childRenderer.NativeView);
        SetNativeControl(Control!);
        ConstructInteraction(e.NewElement.MenuItems);
    }

    private void DeconstructIntercation()
    {
        if (Control is not null && contextMenu is not null)
        {
            Control.RemoveInteraction(contextMenu);
            //contextMenuDelegate?.Dispose();
            //contextMenu?.Dispose();
        }
    }

    private void ConstructInteraction(ContextMenuItems menuItems)
    {
        DeconstructIntercation();
        if (menuItems?.Count > 0)
        {
            contextMenuDelegate = new ContextMenuDelegate(
                menuItems,
                () => TraitCollection.UserInterfaceStyle
            );
            contextMenu = new UIContextMenuInteraction(contextMenuDelegate);
            Control!.AddInteraction(contextMenu);
        }
    }
}
