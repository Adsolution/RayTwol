using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace RayTwol
{
    public static class Gizmos
    {
        public static List<Mesh> gizmo_move = new List<Mesh>();

        public static bool hoverX, hoverY, hoverZ;
        public static bool held;

        public static void Create()
        {
            gizmo_move.Clear();

            Mesh x = Primitives.Mesh_Arrow(0.4f, 2, new Vec3(), new Vec3(0, 0, 90));
            x.colour = new Colour(255, 0, 0);
            x.hidden = true;
            gizmo_move.Add(x);
            Global.Meshes.gizmos.Add(x);

            Mesh y = Primitives.Mesh_Arrow(0.4f, 2, new Vec3(), new Vec3(90, 0, 0));
            y.colour = new Colour(0, 255, 0);
            y.hidden = true;
            gizmo_move.Add(y);
            Global.Meshes.gizmos.Add(y);

            Mesh z = Primitives.Mesh_Arrow(0.4f, 2, new Vec3(), new Vec3(0, 0, 0));
            z.colour = new Colour(0, 0, 255);
            z.hidden = true;
            gizmo_move.Add(z);
            Global.Meshes.gizmos.Add(z);
        }

        public static void SetPos(Vec3 position)
        {
            foreach (Mesh m in gizmo_move)
                m.pos = position;
        }

        public static void SetScale(float scale)
        {
            foreach (Mesh m in gizmo_move)
                m.scale = new Vec3(scale, scale, scale);
        }

        public static void Hide()
        {
            foreach (Mesh m in gizmo_move)
                m.hidden = true;
        }

        public static void Unhide()
        {
            foreach (Mesh m in gizmo_move)
                m.hidden = false;
        }
    }



    partial class MainWindow
    {
        void GizmoAction()
        {
            if (Global.selectedEntity != null && Global.selectedEntity.mesh != null)
            {
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
    }
}
