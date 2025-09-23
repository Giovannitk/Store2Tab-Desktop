using System;
using System.Globalization;
using System.Windows.Data;

namespace Store2Tab.Views
{
    public class ByteToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            try
            {
                var b = System.Convert.ToByte(value, CultureInfo.InvariantCulture);
                return b != 0;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var isChecked = System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                return (byte)(isChecked ? 1 : 0);
            }
            catch
            {
                return (byte)0;
            }
        }
    }
}


