using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using PvPNet;

namespace PvPNetThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModel();
        }
        
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            (this.DataContext as ViewModel).Dispose();
        }
    }
}