using System.Windows.Data;

namespace UsingDotNET.DirMover.Converts;

public class SizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        long size = (long)value;
        string r = size.GetBytesReadable();

        return r;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return value;
    }
}