using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Global.meshes_gizmos.Add(x);

            Mesh y = Primitives.Mesh_Arrow(0.4f, 2, new Vec3(), new Vec3(90, 0, 0));
            y.colour = new Colour(0, 255, 0);
            y.hidden = true;
            gizmo_move.Add(y);
            Global.meshes_gizmos.Add(y);

            Mesh z = Primitives.Mesh_Arrow(0.4f, 2, new Vec3(), new Vec3(0, 0, 0));
            z.colour = new Colour(0, 0, 255);
            z.hidden = true;
            gizmo_move.Add(z);
            Global.meshes_gizmos.Add(z);
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
}
