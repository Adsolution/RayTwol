using System;
using System.Collections.Generic;

namespace RayTwol
{
    public static class Primitives
    {
        public static Mesh Mesh_Cube_tri(float size_x, float size_y, float size_z, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cube = new Mesh(addToGlobalMeshes);
            cube.colour = Colour.RandomColour();
            cube.pos = position;
            cube.rot = rotation;

            float x = size_x / 2;
            float y = size_y / 2;
            float z = size_z / 2;

            cube.AddVert(-x, y, z);
            cube.AddVert(x, y, z);
            cube.AddVert(-x, -y, z);
            cube.AddVert(x, -y, z);
            cube.AddVert(-x, y, -z);
            cube.AddVert(x, y, -z);
            cube.AddVert(x, -y, -z);
            cube.AddVert(-x, -y, -z);

            cube.AddFace(0, 1, 2);
            cube.AddFace(1, 2, 3);
            cube.AddFace(1, 3, 6);
            cube.AddFace(1, 5, 6);
            cube.AddFace(0, 1, 4);
            cube.AddFace(1, 4, 5);
            cube.AddFace(2, 3, 7);
            cube.AddFace(3, 6, 7);
            cube.AddFace(0, 2, 7);
            cube.AddFace(0, 4, 7);
            cube.AddFace(4, 5, 6);
            cube.AddFace(4, 6, 7);

            return cube;
        }
        public static Mesh Mesh_Cube(float size_x, float size_y, float size_z, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cube = new Mesh(addToGlobalMeshes);
            cube.colour = Colour.RandomColour();
            cube.pos = position;
            cube.rot = rotation;

            float x = size_x / 2;
            float y = size_y / 2;
            float z = size_z / 2;

            cube.AddVert(-x, y, z);
            cube.AddVert(x, y, z);
            cube.AddVert(-x, -y, z);
            cube.AddVert(x, -y, z);
            cube.AddVert(-x, y, -z);
            cube.AddVert(x, y, -z);
            cube.AddVert(-x, -y, -z);
            cube.AddVert(x, -y, -z);

            cube.AddFace(0, 1, 3, 2);
            cube.AddFace(4, 5, 7, 6);

            cube.AddFace(0, 1, 5, 4);
            cube.AddFace(2, 3, 7, 6);

            return cube;
        }
        public static Mesh Mesh_Plane(float size_x, float size_z, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cube = new Mesh(addToGlobalMeshes);
            cube.colour = Colour.RandomColour();
            cube.pos = position;
            cube.rot = rotation;

            float x = size_x / 2;
            float z = size_z / 2;

            cube.AddVert(-x, 0, -z);
            cube.AddVert(-x, 0, z);
            cube.AddVert(x, 0, -z);
            cube.AddVert(x, 0, z);

            cube.AddFace(0, 1, 2);
            cube.AddFace(1, 2, 3);

            return cube;
        }
        public static Mesh Mesh_Cyllinder_tri(float radius, float height, int sides, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cyl = new Mesh(addToGlobalMeshes);
            cyl.colour = Colour.RandomColour();
            cyl.pos = position;
            cyl.rot = rotation;

            float r = radius;
            float h = height;
            int s = sides;

            // base verts
            cyl.AddVert(0, h / 2, 0);
            for (float i = 0; i < 2; i += 2f / s)
                cyl.AddVert((float)Math.Cos(Math.PI * i) * r / 2, h / 2, (float)Math.Sin(Math.PI * i) * r / 2);

            // cap verts
            cyl.AddVert(0, -h / 2, 0);
            for (float i = 0; i < 2; i += 2f / s)
                cyl.AddVert((float)Math.Cos(Math.PI * i) * r / 2, -h / 2, (float)Math.Sin(Math.PI * i) * r / 2);

            // base faces
            for (int i = 0; i < s - 1; i++)
                cyl.AddFace(0, i + 1, i + 2);
            cyl.AddFace(s, 1, 0);

            // cap faces
            for (int i = s; i < s * 2; i++)
                cyl.AddFace(s + 1, i + 1, i + 2);
            cyl.AddFace(s * 2 + 1, s + 1, s + 2);

            // joining faces
            for (int i = 1; i < s; i++)
            {
                cyl.AddFace(i, i + 1, i + s + 1);
                cyl.AddFace(i + 1, i + s + 1, i + s + 2);
            }
            cyl.AddFace(1, s, s + 2);
            cyl.AddFace(s, s + 2, s + s + 1);

            return cyl;
        }

        public static Mesh Mesh_Cyllinder(float radius, float height, int sides, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cyl = new Mesh(addToGlobalMeshes);
            cyl.colour = Colour.RandomColour();
            cyl.pos = position;
            cyl.rot = rotation;

            float r = radius;
            float h = height;
            int s = sides;

            // base verts
            for (float i = 0; i < 2; i += 2f / s)
                cyl.AddVert((float)Math.Cos(Math.PI * i) * r / 2, h / 2, (float)Math.Sin(Math.PI * i) * r / 2);

            // cap verts
            for (float i = 0; i < 2; i += 2f / s)
                cyl.AddVert((float)Math.Cos(Math.PI * i) * r / 2, -h / 2, (float)Math.Sin(Math.PI * i) * r / 2);
            
            List<int> bottomFace = new List<int>();
            for (int i = 0; i < s; i++)
                bottomFace.Add(i);
            cyl.AddFace(bottomFace);

            List<int> topFace = new List<int>();
            for (int i = s; i < s * 2; i++)
                topFace.Add(i);
            cyl.AddFace(topFace);

            for (int i = 0; i < s - 1; i++)
                cyl.AddFace(i, i + 1, i + s + 1, i + s);
                
            return cyl;
        }

        public static Mesh Mesh_Cone(float radius, float height, int sides, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cone = new Mesh(addToGlobalMeshes);
            cone.colour = Colour.RandomColour();
            cone.pos = position;
            cone.rot = rotation;

            float r = radius;
            float h = height;
            int s = sides;

            for (float i = 0; i < 2; i += 2f / s)
                cone.AddVert((float)Math.Cos(Math.PI * i) * r / 2, 0, (float)Math.Sin(Math.PI * i) * r / 2);
            cone.AddVert(0, h, 0);

            // base faces
            for (int i = 0; i < s - 1; i++)
                cone.AddFace(0, i + 1, i + 2);
            cone.AddFace(s, 1, 0);

            for (int i = 0; i < s - 1; i++)
                cone.AddFace(i, i + 1, s);

            return cone;
        }

        public static Mesh Mesh_Arrow(float radius, float length, Vec3 position, Vec3 rotation, bool addToGlobalMeshes = true)
        {
            Mesh cyl = Primitives.Mesh_Cyllinder_tri(radius, length, 8, new Vec3(0, length / 2, 0), new Vec3(), false);
            Mesh cone = Primitives.Mesh_Cone(radius * 2.5f, radius * 4, 16, new Vec3(0, length, 0), new Vec3(), false);

            Mesh arrow = new Mesh();
            arrow.colour = Colour.RandomColour();
            arrow.pos = position;
            arrow.rot = rotation;

            arrow.AddMesh(cyl);
            arrow.AddMesh(cone);

            return arrow;
        }
    }
}
