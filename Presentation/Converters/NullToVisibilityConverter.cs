using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RWTree.Presentation.Converters;

/// <summary>
/// Converter that converts null/empty values to Visibility.Collapsed and non-null to Visibility.Visible
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isInverted = parameter?.ToString()?.Equals("Inverted", StringComparison.OrdinalIgnoreCase) == true;
        bool hasValue = value != null;

        if (isInverted)
            hasValue = !hasValue;

        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}