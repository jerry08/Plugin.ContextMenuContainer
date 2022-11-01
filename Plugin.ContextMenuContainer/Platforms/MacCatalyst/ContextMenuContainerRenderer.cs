using System;
using System.Collections.Generic;
using System.Text;
using AppKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.MacOS;

namespace Maui.ContextMenuContainer;

[Preserve(AllMembers = true)]
class ContextMenuContainerRenderer : ViewRenderer<ContextMenuContainer, ContextContainerNativeView>
{
    protected override void OnElementChanged(ElementChangedEventArgs<ContextMenuContainer> e)
    {
        base.OnElementChanged(e);
        if (e.OldElement != null)
        {

        }
        if (e.NewElement == null || e.NewElement.Content == null)
        {
            return;
        }
        var childRenderer = Platform.CreateRenderer(Element.Content);
        var nativeConainer = new ContextContainerNativeView(childRenderer, Element.MenuItems);
        SetNativeControl(nativeConainer);
    }
}