using AcademiaDoZe.Presentation.AppMaui.Helpers;
using System.Globalization;
namespace AcademiaDoZe.Presentation.AppMaui.Converters
{
    public class DateOnlyShortToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            var current = CultureInfo.CurrentCulture;
            var pattern = LocalizationManager.Instance.FormatoDataCurta;
            if (value is DateOnly d)
                return d.ToDateTime(new TimeOnly(0, 0)).ToString(pattern, current);
            if (value is DateTime dt)
                return dt.ToString(pattern, current);
            return value.ToString() ?? string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Não é necessário para exibição; implementar se precisar editar via UI

            return value!;

        }
    }
}