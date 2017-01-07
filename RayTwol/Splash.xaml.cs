using System;
using System.Windows;

namespace RayTwol
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            Editor.RaytwolInit += RaytwolInit;
        }

        void RaytwolInit(object sender, EventArgs e)
        {
            Close();
        }

        void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.Init();
        }


        
    }
}
