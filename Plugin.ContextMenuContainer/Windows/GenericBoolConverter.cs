using System;
using IValueConverter = Microsoft.UI.Xaml.Data.IValueConverter;

namespace Plugin.ContextMenuContainer;

internal class GenericBoolConverter<T> : IValueConverter
{
    public GenericBoolConverter(T @true, T @false)
    {
        True = @true ?? throw new ArgumentNullException(nameof(@true));
        False = @false ?? throw new ArgumentNullException(nameof(@false));
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public T True { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public T False { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var boolean = value as bool?;
        return (boolean ?? false ? True : False)!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
