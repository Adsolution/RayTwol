using System;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Forms.Integration;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace RayTwol
{
    partial class MainWindow : Window
    {
        Stopwatch frameTimer = new Stopwatch();
        Stopwatch frameTimerMin = new Stopwatch();
        Stopwatch frameTimerMax = new Stopwatch();
        public System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();

        GLControl gl;

        public Camera cam = new Camera();
        
        public int width;
        public int height;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Global.viewports.Add(this);

                foreach (FileInfo levelFile in Editor.levelFiles)
                    dropdown_Levels.Items.Add(levelFile.Directory.Name.PadRight(10, ' ') + "•  " + Func.CodeToGameName(levelFile.Directory.Name));

                updateTimer.Interval = 1;
                updateTimer.Start();
                frameTimer.Start();
                frameTimerMin.Start();
                frameTimerMax.Start();

                CamMoved += Viewport_CamMoved;

                updateTimer.Tick += Update;

                Editor.LevelLoad += LevelLoad;
                Memory.checkRunningTimer.Interval = 500;
                Memory.checkRunningTimer.Start();
                Memory.checkRunningTimer.Elapsed += CheckIfGameRunning;
            }
            catch (Exception e)
            {
                var warn = new Warning("Crash", e.Message).ShowDialog();
                Environment.Exit(0);
            }
            
        }

        void viewport_Initialized(object sender, EventArgs e)
        {
            try
            {
                gl = new GLControl(new GraphicsMode(32, 24, 4, 4));
                gl.Load += Gl_Load;
                gl.Resize += Gl_Resize;
                viewport.Child = gl;

                gl.KeyDown += Gl_KeyDown;
                gl.KeyUp += Gl_KeyUp;
                gl.MouseDown += Gl_MouseDown;
                gl.MouseUp += Gl_MouseUp;
                gl.MouseWheel += Gl_MouseWheel;
                gl.MouseMove += Gl_MouseMove;
            }
            catch (Exception ex)
            {
                var warn = new Warning("Crash", ex.Message).ShowDialog();
                Environment.Exit(0);
            }
            
        }
        
        private void Update(object sender, EventArgs e)
        {
            if (Memory.canSync)
                GameSync();
            MouseOverEntity();
            GizmoAction();
            Movement();
            Render();
            FrameRateCounters();
        }

        void FrameRateCounters()
        {
            Global.FPS = 1000f / frameTimer.ElapsedMilliseconds;
            Global.DT = 1f / Global.FPS;
            frameTimer.Restart();

            if (Global.FPS < Global.FPS_min)
                Global.FPS_min = Global.FPS;
            else if (frameTimerMin.ElapsedMilliseconds >= 500)
            {
                Global.FPS_min = Global.FPS;
                frameTimerMin.Restart();
            }

            if (Global.FPS > Global.FPS_max)
                Global.FPS_max = Global.FPS;
            else if (frameTimerMax.ElapsedMilliseconds >= 500)
            {
                Global.FPS_max = Global.FPS;
                frameTimerMax.Restart();
            }

            Global.FPS_avg = (Global.FPS_min + Global.FPS_max) / 2;
        }


        void textbox_MoveSpeed_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                moveSpeed = float.Parse(textbox_MoveSpeed.Text);
                moveSpeed = Global.Clamp(moveSpeed, 1, 999);
            }
            catch { }
            textbox_MoveSpeed.Text = moveSpeed.ToString();
        }
        

        public static EventHandler CamMoved;
        void Viewport_CamMoved(object sender, EventArgs e)
        {
            textbox_CamPosX.Text = cam.pos.x.ToString("0.00");
            textbox_CamPosY.Text = (-cam.pos.z).ToString("0.00");
            textbox_CamPosZ.Text = (-cam.pos.y).ToString("0.00");
        }

        void textbox_CamPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                cam.pos.x = float.Parse(textbox_CamPosX.Text);
            }
            catch
            {
                textbox_CamPosX.Text = cam.pos.x.ToString("0.00");
            }
        }

        void textbox_CamPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                cam.pos.z = -float.Parse(textbox_CamPosY.Text);
            }
            catch
            {
                textbox_CamPosY.Text = (-cam.pos.z).ToString("0.00");
            }
        }

        void textbox_CamPosZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                cam.pos.y = -float.Parse(textbox_CamPosZ.Text);
            }
            catch
            {
                textbox_CamPosZ.Text = (-cam.pos.y).ToString("0.00");
            }
        }

        void objectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Global.selectedEntity = null;
            ListViewItem ent = (ListViewItem)objectsList.SelectedItem;
            int ID = 0;
            if (ent == null)
            {
                textbox_ObjName.Text = "";

                textbox_ObjPosX.Text = "";
                textbox_ObjPosX.IsEnabled = false;
                textbox_ObjPosY.Text = "";
                textbox_ObjPosY.IsEnabled = false;
                textbox_ObjPosZ.Text = "";
                textbox_ObjPosZ.IsEnabled = false;
            }
            else
            {
                ID = (int)ent.Tag;
                Global.selectedEntity = Editor.entities[ID];
                textbox_ObjName.Text = Global.selectedEntity.name;

                if (!Global.selectedEntity.pos.isValid)
                {
                    textbox_ObjPosX.Text = "";
                    textbox_ObjPosX.IsEnabled = false;
                    textbox_ObjPosY.Text = "";
                    textbox_ObjPosY.IsEnabled = false;
                    textbox_ObjPosZ.Text = "";
                    textbox_ObjPosZ.IsEnabled = false;
                }
                else
                {
                    textbox_ObjPosX.Text = (Global.selectedEntity.pos.x).ToString("0.00");
                    textbox_ObjPosX.IsEnabled = true;
                    textbox_ObjPosY.Text = (Global.selectedEntity.pos.y).ToString("0.00");
                    textbox_ObjPosY.IsEnabled = true;
                    textbox_ObjPosZ.Text = (Global.selectedEntity.pos.z).ToString("0.00");
                    textbox_ObjPosZ.IsEnabled = true;
                }
            } 
        }

        void ChangeFocusMode(bool focusable)
        {
            objectsList.Focusable = focusable;
            dropdown_Levels.Focusable = focusable;
            textbox_MoveSpeed.Focusable = focusable;
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        void MouseOverEntity()
        {
            foreach (Mesh mesh in Global.Meshes.entities)
                mesh.colour = new Colour(206, 217, 51);

            if (Global.selectedEntity != null && Global.selectedEntity.mesh != null)
            {
                Global.selectedEntity.mesh.colour = new Colour(255, 100, 255);
            }
            else
            {
                Gizmos.Hide();
                Gizmos.SetPos(new Vec3(100000, 100000, 100000));
            }
        }

        void ClickOnEntity()
        {
            if (Global.mouseOverEntity != null)
            {
                Global.selectedEntity = Global.mouseOverEntity;
                objectsList.SelectedIndex = Global.selectedEntity.ID;
            }

            else
            {
                Global.selectedEntity = null;
                objectsList.SelectedIndex = -1;
            }
        }

        void textbox_ObjPosX_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Global.selectedEntity.pos.isValid)
                    Global.selectedEntity.pos = new Vec3(float.Parse(textbox_ObjPosX.Text), Global.selectedEntity.pos.y, Global.selectedEntity.pos.z);
            }
            catch
            {
                if (Global.selectedEntity != null)
                    textbox_ObjPosX.Text = Global.selectedEntity.pos.x.ToString("0.00");
            }
        }
        void textbox_ObjPosY_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Global.selectedEntity.pos.isValid)
                    Global.selectedEntity.pos = new Vec3(Global.selectedEntity.pos.x, float.Parse(textbox_ObjPosY.Text), Global.selectedEntity.pos.z);
            }
            catch
            {
                if (Global.selectedEntity != null)
                    textbox_ObjPosY.Text = Global.selectedEntity.pos.y.ToString("0.00");
            }
        }
        void textbox_ObjPosZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Global.selectedEntity.pos.isValid)
                    Global.selectedEntity.pos = new Vec3(Global.selectedEntity.pos.x, Global.selectedEntity.pos.y, float.Parse(textbox_ObjPosZ.Text));
            }
            catch
            {
                if (Global.selectedEntity != null)
                    textbox_ObjPosZ.Text = Global.selectedEntity.pos.z.ToString("0.00");
            }
        }
    }
}
