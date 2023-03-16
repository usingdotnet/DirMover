using System.IO;
using System.Windows.Data;
using System.Windows.Media;

namespace UsingDotNET.DirMover.Converts;

public class ExistToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        string dir = (string)value;
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            return new SolidColorBrush(Colors.LightYellow);
        }

        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value;
    }
}