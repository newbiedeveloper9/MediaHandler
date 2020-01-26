using System;
using System.Windows;

namespace Converters
{
    public class BoolVisibilityConverter : MarkuExtensionConverterBase<BoolVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is bool booleanVal))
                throw new ArgumentException();

            var negate = (parameter is string b && !string.IsNullOrEmpty(b));
            return (booleanVal != negate) ? Visibility.Visible : Visibility.Collapsed; //xor
        }

        public override object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
