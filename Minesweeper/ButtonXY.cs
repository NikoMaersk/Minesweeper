using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public class ButtonXY : Button
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ButtonXY(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void SetImage(string path)
        {
            Image image = new Image();
            try
            {
                image.Source = new BitmapImage(new Uri(path, UriKind.Relative));
                image.Height = 30;
                image.Opacity = 0.8;
                this.Content = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }
}
