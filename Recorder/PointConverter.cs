using MT.Controls.Converters;
using System;
using System.Globalization;
using System.Windows;

namespace Recorder
{
    class PointConverter : BaseValueConverter<PointConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Point p)
            {                
                return $"({p.X},{p.Y})";
            }
            return "";
        }
    }
}
