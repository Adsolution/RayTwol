using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace RayTwol
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        Process step1;
        Process step2;
        bool goOn = true;

        System.Timers.Timer progTimer = new System.Timers.Timer();

        public SetupWindow()
        {
            InitializeComponent();
            progTimer.Interval = 1000;
            progTimer.Elapsed += ProgressCheck;
            progTimer.Start();
            
            new Thread(() =>
            {
                var r2lib = new ProcessStartInfo("rayman2lib.exe");
                r2lib.UseShellExecute = false;
                r2lib.CreateNoWindow = true;
                r2lib.Arguments = string.Format("unpackcnt \"{0}\" \"{1}\" \"-png\"", Editor.cf_gameDir + "\\Data\\Textures.cnt", Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures");
                step1 = Process.Start(r2lib);
                r2lib.Arguments = string.Format("exportallmaps \"{0}\" \"{1}\"", Editor.cf_gameDir + "\\Data\\World\\Levels", Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");
                step2 = Process.Start(r2lib);
                
                step1.WaitForExit();
                step2.WaitForExit();
                Setup.MoveTexturesFolder();
                Thread.Sleep(500);

                if (goOn)
                    Dispatcher.Invoke(() =>
                    {
                        DialogResult = true;
                        Close();
                    });

            }).Start();
        }

        void ProgressCheck(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                bar.Value = ((double)Func.DirectorySize(new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol")) / Setup.INST_FOLDER_SIZE) * 170;
            });
        }

        void Window_Closed(object sender, EventArgs e)
        {
            goOn = false;
            progTimer.Stop();

            if (!step1.HasExited)
                step1.Kill();

            if (!step2.HasExited)
                step2.Kill();

            if (Func.DirectorySize(new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol")) != Setup.INST_FOLDER_SIZE)
            {
                var warn = new Warning("Setup failed", "Something went wrong during setup. Press OK to try again.").ShowDialog();
                if (warn.Value)
                    Setup.FirstTimeSetup();
            }
        }
    }
}
