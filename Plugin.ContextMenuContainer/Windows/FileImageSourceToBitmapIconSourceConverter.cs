using System;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Controls;
using IValueConverter = Microsoft.UI.Xaml.Data.IValueConverter;

namespace Plugin.ContextMenuContainer;

class FileImageSourceToBitmapIconSourceConverter : IValueConverter
{
    readonly Uri baseUri = new("ms-appx:///");

    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        IconElement? result = null;
        if (value is not FileImageSource source)
            return result;
        if (!string.IsNullOrWhiteSpace(source.File))
        {
            result = new BitmapIcon() { UriSource = new Uri(baseUri, source.File), };
        }
        return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }

    //void UpdateImageDirectory(FileImageSource fileSource)
    //{
    //    if (fileSource == null || fileSource.File == null)
    //        return;

    //    var currentApp = Application.Current;

    //    if (currentApp == null)
    //        return;

    //    var imageDirectory = currentApp.OnThisPlatform().GetImageDirectory();

    //    if (!string.IsNullOrEmpty(imageDirectory))
    //    {
    //        var filePath = fileSource.File;

    //        var directory = IOPath.GetDirectoryName(filePath);

    //        if (string.IsNullOrEmpty(directory) || !IOPath.GetFullPath(directory).Equals(IOPath.GetFullPath(imageDirectory)))
    //        {
    //            filePath = IOPath.Combine(imageDirectory, filePath);
    //            fileSource.File = filePath;
    //        }
    //    }
    //}
}
