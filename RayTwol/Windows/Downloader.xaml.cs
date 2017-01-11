using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Net;
using System.ComponentModel;

namespace RayTwol
{
    /// <summary>
    /// Interaction logic for Downloader.xaml
    /// </summary>
    public partial class Downloader : Window
    {
        public Downloader()
        {
            InitializeComponent();

            var web = new WebClient();
            
            web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            web.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            web.DownloadFileAsync(new Uri("http://highly-amused.com/raytwol/downloads/extra/LEVELS0.DAT"), Editor.cf_gameDir + "\\Data\\World\\Levels\\LEVELS0.DAT");
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bar.Maximum = (int)e.TotalBytesToReceive / 100;
                bar.Value = (int)e.BytesReceived / 100;
                label_progress.Content = string.Format("{0} / 150 MB", e.BytesReceived / 1000000);
            });
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                DialogResult = true;
                Close();
            });
        } 
    }
}
