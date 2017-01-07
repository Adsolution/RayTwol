using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;

namespace RayTwol
{
    public class Camera
    {
        //public Transform t = new Transform();
        public Vec3 pos = new Vec3();
        public Vec3 rot = new Vec3();

        public float fov = 60;
    }


    public class Mesh
    {
        public string name = "";
        public Vec3 pos;
        public Vec3 rot;
        public Vec3 scale = new Vec3(1, 1, 1);
        public Colour colour = new Colour(255, 255, 255);
        public uint texID;
        public bool hidden;
        public object tag;
        public Entity parentEntity;
        
        public List<Vert> verts = new List<Vert>();
        public List<Face> faces = new List<Face>();


        public Mesh(bool addToGlobalMeshes = true)
        {
            if (addToGlobalMeshes)
                Global.meshes.Add(this);
        }
        // Add vert
        public void AddVert(Vert vert)
        {
            verts.Add(vert);
        }
        public void AddVert(Vec3 pos)
        {
            verts.Add(new Vert(pos));
        }
        public void AddVert(float x = 0, float y = 0, float z = 0)
        {
            verts.Add(new Vert(new Vec3(x, y, z)));
        }
        // Add face
        public void AddFace(Face face)
        {
            faces.Add(face);
        }
        public void AddFace(List<int> verts)
        {
            faces.Add(new Face(verts));
        }
        public void AddFace(int vert1, int vert2, int vert3)
        {
            faces.Add(new Face(vert1, vert2, vert3));
        }
        public void AddFace(int vert1, int vert2, int vert3, int vert4)
        {
            faces.Add(new Face(vert1, vert2, vert3, vert4));
        }
        // Add mesh
        public void AddMesh(Mesh mesh)
        {
            int countHold = verts.Count;

            foreach (Vert vert in mesh.verts)
                AddVert(vert.pos.x + mesh.pos.x, vert.pos.y + mesh.pos.y, vert.pos.z + mesh.pos.z);

            foreach (Face face in mesh.faces)
            {
                Face face2 = new Face(face);
                for (int v = 0; v < face.verts.Count; v++)
                    face2.verts[v] += countHold;
                AddFace(face2);
            }
        }

        /// <summary>
        /// Returns a new instance of the mesh with identical properties.
        /// </summary>
        public Mesh Clone()
        {
            Mesh mesh = new Mesh(false);
            mesh.name = name;
            mesh.colour = colour;
            mesh.pos = pos;
            mesh.rot = rot;
            mesh.scale = scale;

            foreach (Vert v in verts)
                mesh.verts.Add(v.Clone());

            foreach (Face f in faces)
                mesh.faces.Add(f.Clone());

            return mesh;
        }
    }


    public class Vert
    {
        public Vec3 pos = new Vec3();
        public bool culled;
        public Vert(Vec3 pos)
        {
            this.pos.x = pos.x;
            this.pos.y = pos.y;
            this.pos.z = pos.z;
        }

        /// <summary>
        /// Returns a new instance of the vertex with identical properties.
        /// </summary>
        public Vert Clone()
        {
            Vert vert = new Vert(new Vec3());
            vert.pos = pos;
            vert.culled = culled;
            return vert;
        }
    }



    public class Face
    {
        public List<int> verts = new List<int>();
        public List<Vec2> uv = new List<Vec2>();
        public float distFromCamera;
        public int renderOrder;
        public bool hasRendered;
        public bool culled;

        public Face()
        {
        }
        public Face(Face face)
        {
            verts = face.verts;
        }
        public Face(int vert1, int vert2, int vert3)
        {
            verts.Add(vert1);
            verts.Add(vert2);
            verts.Add(vert3);
        }
        public Face(int vert1, int vert2, int vert3, int vert4)
        {
            verts.Add(vert1);
            verts.Add(vert2);
            verts.Add(vert3);
            verts.Add(vert4);
        }
        public Face(List<int> verts)
        {
            this.verts = verts;
        }

        /// <summary>
        /// Returns a new instance of the face with identical properties.
        /// </summary>
        public Face Clone()
        {
            return new Face(verts);
        }
    }








    public class Entity
    {
        public CodeBlock code;
        public int ID;
        public string name;
        public Vec3 _pos;
        public Vec3 pos
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
                if (mesh != null)
                    mesh.pos = new Vec3(-value.x, value.z, value.y);
            }
        }
        public Vec2 screenPos;
        public Mesh mesh;

        public Entity(CodeBlock code, int ID, string name, Vec3 pos)
        {
            this.code = code;
            this.ID = ID;
            this.name = name;
            this.pos = pos;

            if (pos.isValid)
            {
                EntityHandle handle = new EntityHandle(Global.viewports[0], this);
                Mesh entity = Primitives.Mesh_Cube(1, 1, 1, new Vec3(-pos.x, pos.z, pos.y), new Vec3());
                entity.parentEntity = this;
                entity.tag = "entity";
                entity.colour = new Colour(206, 217, 51);
                Global.meshes_entities.Add(entity);
                mesh = entity;
            }
        }

        public void UpdateCode()
        {
            // POSITION
            if (pos.isValid)
            {
                byte[] x = Func.FloatToByte4(pos.x);
                byte[] y = Func.FloatToByte4(pos.y);
                byte[] z = Func.FloatToByte4(pos.z);

                code.code[code.offsets[1] + 0x00] = x[0];
                code.code[code.offsets[1] + 0x01] = x[1];
                code.code[code.offsets[1] + 0x02] = x[2];
                code.code[code.offsets[1] + 0x03] = x[3];

                code.code[code.offsets[1] + 0x04] = y[0];
                code.code[code.offsets[1] + 0x05] = y[1];
                code.code[code.offsets[1] + 0x06] = y[2];
                code.code[code.offsets[1] + 0x07] = y[3];

                code.code[code.offsets[1] + 0x08] = z[0];
                code.code[code.offsets[1] + 0x09] = z[1];
                code.code[code.offsets[1] + 0x0A] = z[2];
                code.code[code.offsets[1] + 0x0B] = z[3];
            }
        }
    }




    public class EntityHandle
    {
        MainWindow viewport;
        public Entity entity;
        public bool mouseOver;
        public bool enabled = true;

        public EntityHandle(MainWindow viewport, Entity entity)
        {
            this.viewport = viewport;
            this.entity = entity;
            Global.entityHandles.Add(this);
        }
    }



    public class CodeBlock
    {
        public byte[] code;
        public int start;
        public int end;
        public int[] offsets;

        public CodeBlock(byte[] code, int start, int end, int[] offsets)
        {
            this.code = code;
            this.start = start;
            this.end = end;
            this.offsets = offsets;
        }

        // -- OFFSETS --

        // 0 = Name
        // 1 = Position
        // 2 = Type
    }
}