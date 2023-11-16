using Microsoft.Maui.Controls;
using XmlnsPrefixAttribute = Microsoft.Maui.Controls.XmlnsPrefixAttribute;

//[assembly: InternalsVisibleTo("Plugin.ContextMenuContainer.Tests")]

[assembly: XmlnsDefinition(
    "https://github.com/jerry08/Plugin.ContextMenuContainer",
    "Plugin.ContextMenuContainer"
)]
[assembly: XmlnsPrefix("https://github.com/jerry08/Plugin.ContextMenuContainer", "cm")]
