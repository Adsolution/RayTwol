using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTwol
{
    public static class Setup
    {
        public const long INST_FOLDER_SIZE = 38341965;
        public const long INST_TEXTURES_FOLDER_SIZE = 19150935;
        public const long INST_LEVELS0_SIZE = 150775808;

        public static bool firstTime;

        public static void SetupChecks()
        {
            // Check for LEVELS0.DAT
            if (!File.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\LEVELS0.DAT"))
            {
                var warn = new Warning("LEVELS0.DAT not found", "This is a retail install of Rayman 2 and requires LEVELS0.DAT, a 150 MB file included in the GOG version. Press OK to download and install the file.").ShowDialog();
                if (warn.Value)
                {
                    var dl = new Downloader();
                    dl.ShowDialog();
                    if ((bool)!dl.DialogResult)
                        Environment.Exit(0);
                } 
                else
                    Environment.Exit(0);
            }
            else if (new FileInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\LEVELS0.DAT").Length != INST_LEVELS0_SIZE)
            {
                var warn = new Warning("LEVELS0.DAT corrupted", "LEVELS0.DAT appears to be corrupted. Press OK to re-download.").ShowDialog();
                if (warn.Value)
                {
                    File.Delete(Editor.cf_gameDir + "\\Data\\World\\Levels\\LEVELS0.DAT");
                    var dl = new Downloader();
                    dl.ShowDialog();
                    if ((bool)!dl.DialogResult)
                        Environment.Exit(0);
                }
                else
                    Environment.Exit(0);
            }

            // Check if RayTwol has been used before
            if (!Directory.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"))
            {
                var warn = new Warning("Setup", "Press OK to begin the first-time setup procedure. Once complete, RayTwol will open.").ShowDialog();
                if (warn.Value)
                    FirstTimeSetup();
            }
            else
            {
                long size = Func.DirectorySize(new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"));
                if (!(size == INST_FOLDER_SIZE || size == INST_TEXTURES_FOLDER_SIZE))
                {
                    var warn = new Warning("Warning", "First-time setup is not complete or setup-related files have been modified. If this error persists, press OK to re-initialise the setup.").ShowDialog();
                    if (warn.Value)
                        FirstTimeSetup();
                }
            }

            try
            {
                new MainWindow().ShowDialog();
            }
            catch (Exception e)
            {
                var warn = new Warning("Crash", e.Message).ShowDialog();
                Environment.Exit(0);
            }
            
        }
        
        
        public static void FirstTimeSetup()
        {
            firstTime = true;
            if (Directory.Exists("Textures.cnt.extracted"))
            {
                Func.DeleteDirectoryRecursive("Textures.cnt.extracted");
                Directory.Delete("Textures.cnt.extracted");
            }
            if (Directory.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"))
                Func.DeleteDirectoryRecursive(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");

            Directory.CreateDirectory(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol");
            Directory.CreateDirectory(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures");

            var sw = new SetupWindow();
            sw.ShowDialog();
            if (!(bool)sw.DialogResult)
                Environment.Exit(0);
        }

        
        public static void MoveTexturesFolder()
        {
            Func.MoveDirectory(new DirectoryInfo("Textures.cnt.extracted"), new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures"));
            Func.DeleteDirectoryRecursive("Textures.cnt.extracted");
            Directory.Delete("Textures.cnt.extracted");
        }
    }
}
