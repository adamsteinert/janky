using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Janky.ViewModels;
using MahApps.Metro.Controls;

namespace Janky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool forbidCancel = true;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += (sender, args) => this.DataContext = new MainViewModel(theInfo);

            this.KeyDown += MainWindow_KeyDown;

            this.Closing += OnClosing;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                forbidCancel = false;
                this.Close();
            }
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            cancelEventArgs.Cancel = forbidCancel;
            this.WindowState = WindowState.Minimized;
        }
    }
}
