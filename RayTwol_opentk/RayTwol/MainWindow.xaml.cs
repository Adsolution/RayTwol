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
using K = System.Windows.Forms.Keys;
using M = System.Windows.Forms.MouseButtons;

namespace RayTwol
{
    partial class MainWindow : Window
    {
        Stopwatch frameTimer = new Stopwatch();
        Stopwatch frameTimerMin = new Stopwatch();
        Stopwatch frameTimerMax = new Stopwatch();
        public System.Windows.Forms.Timer updateTimer = new System.Windows.Forms.Timer();

        GLControl gl;

        public MainWindow()
        {
            InitializeComponent();
            Global.viewports.Add(this);
            InitRaytwolStuff();
            
            updateTimer.Interval = 1;
            updateTimer.Start();
            frameTimer.Start();
            frameTimerMin.Start();
            frameTimerMax.Start();

            CamMoved += Viewport_CamMoved;

            updateTimer.Tick += Update;
        }
        void viewport_Initialized(object sender, EventArgs e)
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

        private void Gl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, gl.Width, gl.Height);
        }



        
        void Gl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == K.W)
                W = true;
            if (e.KeyCode == K.A)
                A = true;
            if (e.KeyCode == K.S)
                S = true;
            if (e.KeyCode == K.D)
                D = true;

            if (e.KeyCode == K.ShiftKey)
                moveSpeedMult = 4;
            if (e.KeyCode == K.ControlKey)
                moveSpeedMult = 1f / 4;

            if (Global.selectedEntity != null)
                if (e.KeyCode == K.G)
                {
                    Vec3 p = Global.selectedEntity.pos;
                    float d = 5;
                    cam.pos = new Vec3(p.x + d, -p.z - 2, -p.y + d);
                    cam.rot = new Vec3(10, 135, 0);
                }

            if (e.KeyCode == K.Q && !Global.wireframe)
                Global.wireframe = true;
            else if (e.KeyCode == K.Q && Global.wireframe)
                Global.wireframe = false;
        }
        void Gl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == K.W)
                W = false;
            if (e.KeyCode == K.A)
                A = false;
            if (e.KeyCode == K.S)
                S = false;
            if (e.KeyCode == K.D)
                D = false;

            if (e.KeyCode == K.ShiftKey)
                moveSpeedMult = 1;
            if (e.KeyCode == K.ControlKey)
                moveSpeedMult = 1;
        }

        bool panCam;
        bool mouseLeft;
        void Gl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == M.Left)
                mouseLeft = true;

            if (e.Button == M.Right)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.NoMove2D;
                panCam = true;
            }
                
        }
        void Gl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == M.Left)
                mouseLeft = false;

            if (e.Button == M.Right)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                panCam = false;
            }
        }

        int wheelDir = 0;
        void Gl_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            wheelDir = 0;
            if (e.Delta > 0)
                wheelDir = 1;
            if (e.Delta < 0)
                wheelDir = -1;
            if (e.Delta == 0)
                wheelDir = 0;

            if (e.Delta != 0 && !mouseLeft)
            {
                if (moveSpeed <= 5)
                    moveSpeed += 1 * wheelDir;
                else if (moveSpeed <= 25)
                    moveSpeed += 2 * wheelDir;
                else if (moveSpeed <= 50)
                    moveSpeed += 5 * wheelDir;
                else if (moveSpeed <= 100)
                    moveSpeed += 10 * wheelDir;
                else if (moveSpeed <= 250)
                    moveSpeed += 25 * wheelDir;
                else if (moveSpeed <= 1000)
                    moveSpeed += 50 * wheelDir;
            }
            moveSpeed = Global.Clamp(moveSpeed, 1, 999);
            textbox_MoveSpeed.Text = moveSpeed.ToString();




            if (mouseLeft)
            {
                if (Gizmos.hoverX)
                {
                    Global.selectedEntity.pos = new Vec3(Global.selectedEntity.pos.x + wheelDir * moveSpeedMult, Global.selectedEntity.pos.y, Global.selectedEntity.pos.z);
                    textbox_ObjPosX.Text = Global.selectedEntity.pos.x.ToString("0.00");
                }
                if (Gizmos.hoverY)
                {
                    Global.selectedEntity.pos = new Vec3(Global.selectedEntity.pos.x, Global.selectedEntity.pos.y + wheelDir * moveSpeedMult, Global.selectedEntity.pos.z);
                    textbox_ObjPosY.Text = Global.selectedEntity.pos.y.ToString("0.00");
                }
                if (Gizmos.hoverZ)
                {
                    Global.selectedEntity.pos = new Vec3(Global.selectedEntity.pos.x, Global.selectedEntity.pos.y, Global.selectedEntity.pos.z + wheelDir * moveSpeedMult);
                    textbox_ObjPosZ.Text = Global.selectedEntity.pos.z.ToString("0.00");
                }

            }
        }

        Vec2 mousePosPrev = new Vec2();
        Vec2 mouseMoveVec = new Vec2();
        void Gl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseMoveVec = new Vec2((e.X - mousePosPrev.x) / 4, (e.Y - mousePosPrev.y) / 4);

            if (panCam)
                cam.rot = new Vec3(
                        Global.Clamp(cam.rot.x + mouseMoveVec.y, -90, 90),
                        cam.rot.y + mouseMoveVec.x,
                        cam.rot.z);
            
            mousePosPrev = new Vec2(e.X, e.Y);
        }







        private void Gl_Load(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Texture2D);

            GL.ClearColor(new Color4(10, 13, 14, 255));

            //GL.Enable(EnableCap.Fog);
            //GL.Fog(FogParameter.FogColor, new float[] { 8, 11, 12 });
            //GL.Fog(FogParameter.FogDensity, 0.1f);
            //GL.Hint(HintTarget.FogHint, HintMode.Nicest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void Update(object sender, EventArgs e)
        {
            Global.polyCount = Global.GetPolyCount(Global.meshes);
            Global.vertCount = Global.GetVertCount(Global.meshes);
            Global.meshCount = Global.GetMeshCount(Global.meshes);

            MouseOverEntity();
            Movement();
            Render();
            DebugText();
            FrameRateCounters();
        }

        Matrix4 projectionMatrix;
        void DoMatrixTransforms(Mesh mesh)
        {
            GL.LoadIdentity();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, (float)gl.Width / gl.Height, 0.1f, 25000);
            GL.MultMatrix(ref projectionMatrix);

            GL.Rotate(cam.rot.x, 1, 0, 0);
            GL.Rotate(cam.rot.y, 0, 1, 0);
            GL.Rotate(cam.rot.z, 0, 0, 1);
            GL.Translate(cam.pos.x, cam.pos.y, cam.pos.z);

            GL.Translate(mesh.pos.x, mesh.pos.y, mesh.pos.z);
            GL.Rotate(mesh.rot.x, 1, 0, 0);
            GL.Rotate(mesh.rot.y, 0, 1, 0);
            GL.Rotate(mesh.rot.z, 0, 0, 1);
            //GL.Scale(mesh.scale.x, mesh.scale.y, mesh.scale.z);
        }


        void GetScreenPosPre()
        {
            GL.LoadIdentity();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(1.2f, (float)gl.Width / gl.Height, 0.1f, 25000);
            GL.MultMatrix(ref projectionMatrix);

            GL.Rotate(cam.rot.x, 1, 0, 0);
            GL.Rotate(cam.rot.y, 0, 1, 0);
            GL.Rotate(cam.rot.z, 0, 0, 1);
            GL.Translate(cam.pos.x, cam.pos.y, cam.pos.z);
        }

        Vec2 GetScreenPos(Vector3 sPos)
        {
            Matrix4 view;
            GL.GetFloat(GetPName.ModelviewMatrix, out view);

            sPos = Vector3.Transform(sPos, view);
            sPos = Vector3.Transform(sPos, projectionMatrix);

            return new Vec2(sPos.X, sPos.Y);
        }



        void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            // ENTITIES
            GL.LineWidth(3);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            foreach (Mesh mesh in Global.meshes_entities)
            {
                GL.Color3((float)mesh.colour.r / 255, (float)mesh.colour.g / 255, (float)mesh.colour.b / 255);

                DoMatrixTransforms(mesh);

                foreach (Face face in mesh.faces)
                {
                    GL.Begin(BeginMode.Quads);
                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y, mesh.verts[face.verts[0]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y, mesh.verts[face.verts[1]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y, mesh.verts[face.verts[2]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[3]].pos.x, mesh.verts[face.verts[3]].pos.y, mesh.verts[face.verts[3]].pos.z);
                    GL.End();
                }
            }
            /*
            GetScreenPosPre();
            foreach (Mesh mesh in Global.meshes_entities)
            {
                mesh.parentEntity.screenPos = GetScreenPos(new Vector3(mesh.pos.x, mesh.pos.y, mesh.pos.z));
                Console.WriteLine(Editor.entities[3].screenPos.x.ToString() + ", " + Editor.entities[3].screenPos.y.ToString());
            }*/


            // WORLD opaque
            GL.LineWidth(1);
            if (Global.wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Color3(0.5f, 0.5f, 0.5f);
            foreach (Mesh mesh in Global.meshes_world_opaque)
            {
                DoMatrixTransforms(mesh);
                foreach (Face face in mesh.faces)
                {
                    GL.BindTexture(TextureTarget.Texture2D, Global.texture[mesh.texID]);
                    GL.Begin(BeginMode.Triangles);

                    GL.TexCoord2(face.uv[0].x, face.uv[0].y);
                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y, mesh.verts[face.verts[0]].pos.z);

                    GL.TexCoord2(face.uv[1].x, face.uv[1].y);
                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y, mesh.verts[face.verts[1]].pos.z);

                    GL.TexCoord2(face.uv[2].x, face.uv[2].y);
                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y, mesh.verts[face.verts[2]].pos.z);

                    GL.End();
                }
            }

            
            // WORLD transparent
            foreach (Mesh mesh in Global.meshes_world_transparent)
            {
                DoMatrixTransforms(mesh);
                foreach (Face face in mesh.faces)
                {
                    GL.BindTexture(TextureTarget.Texture2D, Global.texture[mesh.texID]);
                    GL.Begin(BeginMode.Triangles);
                    
                    GL.TexCoord2(face.uv[0].x, face.uv[0].y);
                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y, mesh.verts[face.verts[0]].pos.z);
                    
                    GL.TexCoord2(face.uv[1].x, face.uv[1].y);
                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y, mesh.verts[face.verts[1]].pos.z);
                    
                    GL.TexCoord2(face.uv[2].x, face.uv[2].y);
                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y, mesh.verts[face.verts[2]].pos.z);

                    GL.End();
                }
            }





            // GIZMOS
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            foreach (Mesh mesh in Global.meshes_gizmos)
            {
                GL.Color3((float)mesh.colour.r / 255, (float)mesh.colour.g / 255, (float)mesh.colour.b / 255);

                DoMatrixTransforms(mesh);

                foreach (Face face in mesh.faces)
                {
                    GL.Begin(BeginMode.Triangles);
                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y, mesh.verts[face.verts[0]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y, mesh.verts[face.verts[1]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y, mesh.verts[face.verts[2]].pos.z);
                    GL.End();
                }
            }




            /*
            // -- Get render order (furthest to nearest)
            foreach (Mesh mesh in Global.meshes_world_transparent)
                foreach (Face face in mesh.faces)
                    face.distFromCamera = Vec3.dist(mesh.verts[face.verts[0]].pos, cam.pos);

            int tFaces = (int)Global.GetPolyCount(Global.meshes_world_transparent);
            float tempDist = float.MaxValue;
            for (int i = 0; i < tFaces; i++)
            {
                float nextFurthest = 0;
                foreach (Mesh mesh in Global.meshes_world_transparent)
                    foreach (Face face in mesh.faces)
                        if (face.distFromCamera > nextFurthest && face.distFromCamera < tempDist)
                        {
                            nextFurthest = face.distFromCamera;
                            face.renderOrder = i;
                        }
                tempDist = nextFurthest;
            }
            
            // -- Draw transparent meshes
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Color3(0.5f, 0.5f, 0.5f);
                for (int i = 0; i < tFaces; i++)
                    foreach (Mesh mesh in Global.meshes)
                        if (mesh.tag as string == "world")
                        {
                            DoMatrixTransforms(mesh);

                            foreach (Face face in mesh.faces)
                                if (face.renderOrder == i)
                                {
                                    GL.BindTexture(TextureTarget.Texture2D, Global.texture[mesh.texID]);
                                    GL.Begin(BeginMode.Triangles);

                                    if (face.uv.Count > 0)
                                        GL.TexCoord2(face.uv[0].x, face.uv[0].y);
                                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y, mesh.verts[face.verts[0]].pos.z);

                                    if (face.uv.Count > 1)
                                        GL.TexCoord2(face.uv[1].x, face.uv[1].y);
                                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y, mesh.verts[face.verts[1]].pos.z);

                                    if (face.uv.Count > 2)
                                        GL.TexCoord2(face.uv[2].x, face.uv[2].y);
                                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y, mesh.verts[face.verts[2]].pos.z);

                                    GL.End();
                                }
                        }
            */

            GL.Flush();
            gl.SwapBuffers();
        }
        



        


        void DebugText()
        {/*
            if (Global.displayDebug)
                label_Debug.Content = string.Format("FPS: {0}", Global.FPS_avg.ToString("0.0"), Global.vertCount, Global.polyCount);
            else
                label_Debug.Content = "";*/
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

        
            
        static float moveSpeed = 40;
        static float moveSpeedMult = 1;
        bool W, A, S, D;
        void Movement()
        {
            if (W)
                cam.pos += Direction.Forward(cam.rot) * moveSpeed * moveSpeedMult * Global.DT;
            if (S)
                cam.pos += Direction.Backward(cam.rot) * moveSpeed * moveSpeedMult * Global.DT;
            if (A)
                cam.pos += Direction.Left(cam.rot) * moveSpeed * moveSpeedMult * Global.DT;
            if (D)
                cam.pos += Direction.Right(cam.rot) * moveSpeed * moveSpeedMult * Global.DT;
            CamMoved(this, EventArgs.Empty);
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
        {/*
            foreach (EntityHandle eh in Global.entityHandles)
            {
                foreach (EntityHandle eh2 in Global.entityHandles)
                    eh2.entity.mesh.colour = new Colour(206, 217, 51);

                Vec3 pos = GetScreenPos(cam, new Vec3(eh.entity.pos.x, eh.entity.pos.z, eh.entity.pos.y));
                float al = height / pos.z + 10;

                if (mousePosReal.x > pos.x - al && mousePosReal.x < pos.x + al && mousePosReal.y > pos.y - al && mousePosReal.y < pos.y + al)
                {
                    Global.mouseOverEntity = eh.entity;

                    eh.entity.mesh.colour = new Colour(255, 102, 0);

                    textblock_obj_name.Margin = new Thickness(mousePos.x, mousePos.y + 20, 0, 0);
                    textblock_obj_name.Text = eh.entity.name;

                    textblock_obj_pos.Margin = new Thickness(mousePos.x, mousePos.y + 40, 0, 0);
                    //textblock_obj_pos.Text = eh.entity.t.pos.x.ToString("0.00") + ",  " + eh.entity.t.pos.y.ToString("0.00") + ",  " + eh.entity.t.pos.z.ToString("0.00");

                    break;
                }
                else
                {
                    Global.mouseOverEntity = null;
                    eh.entity.mesh.colour = new Colour(206, 217, 51);
                    textblock_obj_name.Text = "";
                    textblock_obj_pos.Text = "";
                }
            }
            */

            foreach (Mesh mesh in Global.meshes_entities)
                mesh.colour = new Colour(206, 217, 51);

            if (Global.selectedEntity != null && Global.selectedEntity.mesh != null)
            {
                Global.selectedEntity.mesh.colour = new Colour(255, 100, 255);


                
                if (mouseLeft)
                    Gizmos.held = true;
                else
                    Gizmos.held = false;
                
                Gizmos.SetPos(Global.selectedEntity.mesh.pos);
                Gizmos.SetScale(Vec3.dist(cam.pos, Global.selectedEntity.mesh.pos) / 40);
                
                int c = 0;
                GL.ReadPixels((int)mousePosPrev.x, (int)(mousePosPrev.y * -1 + viewport.ActualHeight), 1, 1, PixelFormat.Rgb, PixelType.UnsignedByte, ref c);
                byte r = (byte)(c >> 0);
                byte g = (byte)(c >> 8);
                byte b = (byte)(c >> 16);
                
                if (!Gizmos.held)
                {
                    if ((r == 255 && g == 0 && b == 0) || (r == 255 && g == 150 && b == 150))
                    {
                        Gizmos.hoverX = true;
                        Gizmos.gizmo_move[0].colour = new Colour(255, 150, 150);
                    }
                    else
                    {
                        Gizmos.hoverX = false;
                        Gizmos.gizmo_move[0].colour = new Colour(255, 0, 0);
                    }


                    if ((r == 0 && g == 255 && b == 0) || (r == 200 && g == 255 && b == 200))
                    {
                        Gizmos.hoverY = true;
                        Gizmos.gizmo_move[1].colour = new Colour(200, 255, 200);
                    }
                    else
                    {
                        Gizmos.hoverY = false;
                        Gizmos.gizmo_move[1].colour = new Colour(0, 255, 0);
                    }


                    if ((r == 0 && g == 0 && b == 255) || (r == 120 && g == 120 && b == 255))
                    {
                        Gizmos.hoverZ = true;
                        Gizmos.gizmo_move[2].colour = new Colour(120, 120, 255);
                    }
                    else
                    {
                        Gizmos.hoverZ = false;
                        Gizmos.gizmo_move[2].colour = new Colour(0, 0, 255);
                    }
                }
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

        void button_Help_Click(object sender, RoutedEventArgs e)
        {
            if (!Global.viewingHelp)
            {
                Global.help = new Help();
                Global.help.Show();
            }
            else
                Global.help.Focus();
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
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
