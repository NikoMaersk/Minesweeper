using System;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for ModalDialogBox.xaml
    /// </summary>
    public partial class ModalDialogBox : UserControl
    {
        public event EventHandler NewGameClicked;

        public ModalDialogBox()
        {
            InitializeComponent();
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGameClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
