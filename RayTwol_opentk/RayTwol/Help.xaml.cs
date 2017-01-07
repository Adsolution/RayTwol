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
using System.IO;

namespace RayTwol
{
    /// <summary>
    /// Interaction logic for Help.xaml
    /// </summary>
    public partial class Help : Window
    {
        public Help()
        {
            InitializeComponent();

            if (File.Exists("readme.txt"))
            {
                var readme = File.OpenText("readme.txt");
                textBlock.Text = readme.ReadToEnd();
            }
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Global.viewingHelp = false;
        }

        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Global.viewingHelp = true;
        }
    }
}
