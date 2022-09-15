using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Coosu.Shared;

namespace Coosu.Storyboard.Storybrew.Text;

class DebugConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IList<char> list)
            return string.Join("", list);
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
            return s.Where(k => k > 31 && k != 127).Distinct().ToArray();
        return EmptyArray<char>.Value;
    }
}