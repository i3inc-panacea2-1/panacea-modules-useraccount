using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Panacea.Modules.UserAccount.Converters
{
    public class NotBooleanToVisibilityConverter : IValueConverter
    {
        BooleanToVisibilityConverter _converter;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _converter = _converter ?? new BooleanToVisibilityConverter();
            return _converter.Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
