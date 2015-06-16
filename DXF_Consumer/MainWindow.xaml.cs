using Microsoft.Win32;
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

namespace DXF_Consumer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void OpenDxf(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();

            f.ShowDialog();

            string dxfName = f.FileName;

            if(dxfName != "")
            {
                Runner window = new Runner(dxfName);
                window.Show();
            }
        }

        private void ArtSearch(object sender, RoutedEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            List<Runner> windows = new List<Runner>();
            f.Multiselect = true;

            f.ShowDialog();

            foreach(string path in f.FileNames)
            {
                Runner current = new Runner(path);
                current.Show();
                windows.Add(current);
            }
        }
    }
}
