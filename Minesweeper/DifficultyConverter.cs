using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    public class DifficultyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Difficulty difficulty)
            {
                return difficulty.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string difficultyString)
            {
                if (Enum.TryParse(difficultyString, out Difficulty difficulty))
                {
                    return difficulty;
                }
            }
            else if (value is ComboBoxItem comboBoxItem)
            {
                if (Enum.TryParse(comboBoxItem.Content.ToString(), out Difficulty difficulty))
                {
                    return difficulty;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
