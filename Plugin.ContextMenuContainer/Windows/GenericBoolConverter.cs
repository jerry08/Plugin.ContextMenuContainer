using System;
using IValueConverter = Microsoft.UI.Xaml.Data.IValueConverter;

namespace Plugin.ContextMenuContainer;

class GenericBoolConverter<T> : IValueConverter
{
    public T True { get; set; }

    public T False { get; set; }

    public GenericBoolConverter(T True, T False)
    {
        this.True = True ?? throw new ArgumentNullException(nameof(True));
        this.False = False ?? throw new ArgumentNullException(nameof(False));
    }

    public object? Convert(object? value, Type targetType, object parameter, string language)
    {
        var boolean = value as bool?;
        return (boolean ?? false) ? True : False;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotSupportedException();
    }
}
