using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
