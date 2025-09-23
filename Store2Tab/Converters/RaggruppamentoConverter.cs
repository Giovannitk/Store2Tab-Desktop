using System;
using System.Globalization;
using System.Windows.Data;

namespace Store2Tab.Converters
{
    public class RaggruppamentoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte raggruppamento)
            {
                return raggruppamento switch
                {
                    0 => "un Pass. per S, V e P (unico numero)",
                    1 => "Pass. per S, V e P (unico pass.)",
                    2 => "un Pass. per S, V e P",
                    4 => "un Pass. per S, V e Cod. Trac.",
                    _ => ""
                };
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Non necessario per il display
            throw new NotImplementedException();
        }
    }
}