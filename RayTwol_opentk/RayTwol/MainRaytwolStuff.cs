using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace RayTwol
{
    partial class MainWindow
    {
        void InitRaytwolStuff()
        {
            Editor.LevelLoad += LevelLoad;

            foreach (FileInfo levelFile in Editor.levelFiles)
                dropdown_Levels.Items.Add(levelFile.Directory.Name.PadRight(10, ' ') + "•  " + Func.CodeToGameName(levelFile.Directory.Name));
            

            // FIRST TIME EXTRACTIONS
            if (!Directory.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"))
            {
                Func.FirstTimeSetup();
            }
            if (Directory.Exists(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"))
            {
                long size = Func.DirectorySize(new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol"));
                if (!(size == 38341965 || size == 19150935))
                {
                    var warn = new Warning("Warning", "First-time setup is not complete or setup-related files have been modified. If this error persists, press OK to re-initialise the setup.").ShowDialog();
                    if (warn.Value)
                        Func.FirstTimeSetup();
                    Application.Current.Shutdown();
                }
            }
                
            if (Directory.Exists("Textures.cnt.extracted"))
            {
                Func.MoveDirectory(new DirectoryInfo("Textures.cnt.extracted"), new DirectoryInfo(Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\Textures"));
                Func.DeleteDirectoryRecursive("Textures.cnt.extracted");
                Directory.Delete("Textures.cnt.extracted");

                Global.help = new Help();
                Global.help.Show();
            }
        }

        void LevelLoad(object sender, EventArgs e)
        {
            objectsList.SelectedIndex = -1;
            objectsList.Items.Clear();
            label_EntityCount.Content = Editor.entities.Count.ToString() + "  Objects found";

            // Update objects list
            foreach (Entity entity in Editor.entities)
            {
                ListViewItem ent = new ListViewItem();
                ent.Content = entity.name;
                ent.Tag = entity.ID;
                objectsList.Items.Add(ent);
            }
            Gizmos.Create();
        }







        // CHANGE LEVEL
        void dropdown_Levels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.OpenLevel(Editor.levelFiles[dropdown_Levels.SelectedIndex]);
        }
        

        // FILE
        void button_Revert_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.currLevel != null)
                Editor.OpenLevel(Editor.currLevel, "_ORIG");
        }
        
        void button_Fetch_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.currLevel != null)
                Editor.OpenLevel(Editor.currLevel, "_HOLD");
        }

        void button_Hold_Click(object sender, RoutedEventArgs e)
        {
            if (Editor.currLevel != null)
                Editor.SaveLevel("_HOLD");
        }

        void button_Save_click(object sender, RoutedEventArgs e)
        {
            if (Editor.currLevel != null)
                Editor.SaveLevel();
        }


        // RUN GAME
        private void button_Run_Click(object sender, RoutedEventArgs e)
        {
            var r2 = new ProcessStartInfo();
            r2.WorkingDirectory = Editor.cf_gameDir;
            r2.FileName = "Rayman2.exe";
            Process.Start(r2);
        }
    }
}
