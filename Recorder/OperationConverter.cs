using MT.Controls.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Recorder
{
    class KeyConverter : BaseValueConverter<KeyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Key k)
            {
                return $"{k}";
            }
            return "";
        }
    }
}
