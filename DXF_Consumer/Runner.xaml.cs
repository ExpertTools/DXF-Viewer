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
using System.Windows.Shapes;

namespace DXF_Consumer
{
    /// <summary>
    /// Interaction logic for Runner.xaml
    /// </summary>
    public partial class Runner : Window
    {
        public Runner(string source)
        {
            InitializeComponent();
            main.DxfStart(source);
        }
    }
}
