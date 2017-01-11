using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Globalization;
using System.Timers;
using System.Windows;

namespace RayTwol
{
    public class Global
    {
        public static bool debugMode = true;

        public static class Meshes
        {
            public static List<Mesh> all = new List<Mesh>();
            public static List<Mesh> world_opaque = new List<Mesh>();
            public static List<Mesh> world_transparent = new List<Mesh>();
            public static List<Mesh> entities = new List<Mesh>();
            public static List<Mesh> gizmos = new List<Mesh>();

            public static Mesh rayman = Primitives.Mesh_Cube(1, 1.5f, 1, new Vec3(), new Vec3());
            public static Mesh camera = Primitives.Mesh_Cyllinder_tri(0.5f, 1, 5, new Vec3(), new Vec3());
        }

        public static bool viewingHelp;
        public static Help help = new Help();

        public static bool wireframe;
        
        public static uint[] texture = new uint[1024];
        public static uint startIndex;
        public static int textureCount;

        public static Entity mouseOverEntity;
        public static Entity selectedEntity;

        public static uint meshCount;
        public static uint polyCount;
        public static uint vertCount;

        public static bool displayDebug = true;
        public static float FPS;
        public static float FPS_avg;
        public static float FPS_min;
        public static float FPS_max;
        public static float DT; // deltaTime

        public static bool fog_enabled = true;
        public static float fog_min_visibility = 0.1f;
        public static Colour fog_hold = new Colour(8, 11, 12);
        public static Colour fog_colour = new Colour(0, 0, 0);
        public static float fog_density = 0.01f;


       
        public static uint GetPolyCount(List<Mesh> meshGroup)
        {
            uint tris = 0;
            foreach (Mesh mesh in meshGroup)
                foreach (Face face in mesh.faces)
                    tris++;
            return tris;
        }
        public static uint GetVertCount(List<Mesh> meshGroup)
        {
            uint verts = 0;
            foreach (Mesh mesh in meshGroup)
                foreach (Vert vert in mesh.verts)
                    verts++;
            return verts;
        }
        public static uint GetMeshCount(List<Mesh> meshGroup)
        {
            return (uint)meshGroup.Count;
        }

        
        public static void ClearAllMeshes()
        {
            Meshes.all.Clear();
            Meshes.world_opaque.Clear();
            Meshes.world_transparent.Clear();
            Meshes.entities.Clear();
            Meshes.gizmos.Clear();
        }


        public static float Clamp(float n, float min, float max)
        {
            if (n < min)
                return min;
            else if (n > max)
                return max;
            else
                return n;
        }





        
        public static void LoadSceneFromFolder(string folderName)
        {
            try
            {
                if (Directory.Exists(folderName))
                {
                    // ------ LOAD GEOMETRY ------ //
                    string[] OBJs = Directory.GetFiles(folderName, "*.obj");
                    foreach (string OBJ in OBJs)
                    {
                        StreamReader obj = new StreamReader(OBJ);
                        char[] spl = new char[] { ' ' };
                        char[] splFace = new char[] { '/' };
                        string[] line;
                        Mesh mesh = new Mesh();
                        List<Vec2> vt = new List<Vec2>();
                        mesh.tag = "world";
                        mesh.colour = new Colour(37, 95, 75);

                        while (!obj.EndOfStream)
                        {
                            line = obj.ReadLine().Split(spl);
                            switch (line[0])
                            {
                                case "v":
                                    mesh.AddVert(float.Parse(line[1], CultureInfo.InvariantCulture), float.Parse(line[2], CultureInfo.InvariantCulture), float.Parse(line[3], CultureInfo.InvariantCulture));
                                    break;

                                case "vt":
                                    vt.Add(new Vec2(float.Parse(line[1], CultureInfo.InvariantCulture), float.Parse(line[2], CultureInfo.InvariantCulture)));
                                    break;

                                case "f":
                                    Face face = new Face();
                                    for (int i = 1; i < line.Length; i++)
                                    {
                                        string[] fDat = line[i].Split(splFace);
                                        for (int p = 0; p < fDat.Length; p++)
                                            switch (p)
                                            {
                                                case 0:
                                                    face.verts.Add(int.Parse(fDat[0]) - 1);
                                                    break;
                                                case 1:
                                                    face.uv.Add(vt[int.Parse(fDat[1]) - 1]);
                                                    break;
                                            }
                                    }

                                    if (face.verts.Count > 2)
                                        mesh.AddFace(face);
                                    break;
                            }
                        }
                        obj.Close();
                    }

                    // ------ LOAD TEXTURE ------ //
                    string[] MTLs = Directory.GetFiles(folderName, "*.mtl");
                    uint texID = 0;

                    GL.GenTextures(texture.Length, texture);

                    foreach (string MTL in MTLs)
                    {
                        StreamReader mtl = new StreamReader(MTL);
                        char[] spl = new char[] { ' ' };
                        string[] splMat = new string[] { @"..\", ".png" };
                        char[] splName = new char[] { @"\"[0] };
                        string[] line;
                        string texDir = "";
                        string texName;

                        while (!mtl.EndOfStream)
                        {
                            line = mtl.ReadLine().Split(spl);
                            switch (line[0])
                            {
                                case "map_Kd":
                                    if (line[1].Split(splMat, StringSplitOptions.None).Length > 2)
                                        texDir = Editor.cf_gameDir + "\\Data\\World\\Levels\\_raytwol\\" + line[1].Split(splMat, StringSplitOptions.None)[1] + ".png";
                                    break;
                            }
                        }
                        mtl.Close();
                        texName = texDir.Split(splName)[texDir.Split(splName).Length - 1];

                        bool transparent = false;
                        if (texDir != "")
                        {
                            Bitmap textureImage = new Bitmap(Image.FromFile(texDir));
                            textureImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

                            // -- Check cache --
                            bool texFound = false;
                            if (File.Exists("RayTwol_texcache.txt"))
                            {
                                var cache = new StreamReader("RayTwol_texcache.txt");
                                while (!cache.EndOfStream)
                                {
                                    string[] l = cache.ReadLine().Split(spl);
                                    if (l[0] == texName)
                                    {
                                        texFound = true;
                                        if (l[1] == "tr")
                                            transparent = true;
                                        else if (l[1] == "op")
                                            transparent = false;
                                    }
                                }
                                cache.Close();
                            }
                            if (!texFound)
                            {
                                transparent = Func.CheckIfTransparent(textureImage);
                                var cache = new StreamWriter("RayTwol_texcache.txt", true);
                                if (transparent)
                                    cache.WriteLine(texName + " tr");
                                else
                                    cache.WriteLine(texName + " op");
                                cache.Close();
                            }

                            // -- OpenGL --
                            GL.BindTexture(TextureTarget.Texture2D, texture[texID]);
                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureImage.Width, textureImage.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte,
                                textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb).Scan0);

                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMinFilter.Linear);
                        }
                        Meshes.all[(int)texID].texID = texID;

                        if (transparent)
                            Meshes.world_transparent.Add(Meshes.all[(int)texID]);
                        else
                            Meshes.world_opaque.Add(Meshes.all[(int)texID]);

                        texID++;
                    }
                    textureCount = (int)texID;
                }
            }
            catch (Exception e)
            {
                var warn = new Warning("Error", string.Format("The geometry for this level could not be loaded ({0}). Press OK to re-initialise, or Cancel to continue.", e.Message)).ShowDialog();
                if (warn.Value)
                {
                    Setup.FirstTimeSetup();
                    Environment.Exit(0);
                }
            }
        }
    }
}
