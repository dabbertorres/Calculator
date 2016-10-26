using System;
using System.Globalization;
using System.Windows.Data;

namespace Calculator
{
	public class FontSizeAdjuster : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double scale = 1.0;
			if(value.GetType() == typeof(double) && double.TryParse(parameter as string, out scale))
			{
				var dv = (double)value;
				
				return Math.Floor(dv * scale);
			}
			else
			{
				return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double scale = 1.0;
			if (value.GetType() == typeof(double) && double.TryParse(parameter as string, out scale))
			{
				var dv = (double)value;

				return Math.Floor(dv * 1.0 / scale);
			}
			else
			{
				return null;
			}
		}
	}
}
