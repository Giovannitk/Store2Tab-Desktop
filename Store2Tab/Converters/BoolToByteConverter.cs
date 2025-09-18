using System;
using System.Globalization;
using System.Windows.Data;

namespace Store2Tab.Converters
{
    public class BoolToByteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte byteValue)
            {
                return byteValue != 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return (byte)(boolValue ? 1 : 0);
            }
            // IMPORTANTE: Se il valore è null o non valido, restituisco sempre 0
            return (byte)0;
        }
    }
}