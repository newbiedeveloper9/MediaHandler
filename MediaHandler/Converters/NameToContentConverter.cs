﻿using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

public class NameToContentConverter : MarkuExtensionConverterBase<NameToContentConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return string.Empty;

        var userControl = Type.GetType(Assembly.GetExecutingAssembly().GetName().Name + "." + value, null, null);
        return Activator.CreateInstance(userControl);
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
