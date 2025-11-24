using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                return new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return new DateOnly(dateTime.Year, dateTime.Month, dateTime.Day);
            }
            return value;
        }
    }
}
