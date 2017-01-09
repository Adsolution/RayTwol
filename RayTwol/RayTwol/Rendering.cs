using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace RayTwol
{
    partial class MainWindow
    {
        void Gl_Load(object sender, EventArgs e)
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

        void Gl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, gl.Width, gl.Height);
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

            // RAYMAN
            if (Memory.isSynced)
            {
                GL.LineWidth(3);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.BindTexture(TextureTarget.Texture2D, 0);

                Mesh mesh = Global.Meshes.rayman;
                mesh.pos = new Vec3(-Memory.rayPos.x, Memory.rayPos.z, Memory.rayPos.y);
                GL.Color3((float)255 / 255, (float)127 / 255, (float)0 / 255);

                DoMatrixTransforms(mesh);

                foreach (Face face in mesh.faces)
                {
                    GL.Begin(BeginMode.Quads);
                    GL.Vertex3(mesh.verts[face.verts[0]].pos.x, mesh.verts[face.verts[0]].pos.y + 0.75f, mesh.verts[face.verts[0]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[1]].pos.x, mesh.verts[face.verts[1]].pos.y + 0.75f, mesh.verts[face.verts[1]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[2]].pos.x, mesh.verts[face.verts[2]].pos.y + 0.75f, mesh.verts[face.verts[2]].pos.z);
                    GL.Vertex3(mesh.verts[face.verts[3]].pos.x, mesh.verts[face.verts[3]].pos.y + 0.75f, mesh.verts[face.verts[3]].pos.z);
                    GL.End();
                }
            }


            // ENTITIES
            GL.LineWidth(3);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            foreach (Mesh mesh in Global.Meshes.entities)
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


            // WORLD opaque
            GL.LineWidth(1);
            if (Global.wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Color3(0.5f, 0.5f, 0.5f);
            foreach (Mesh mesh in Global.Meshes.world_opaque)
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
            foreach (Mesh mesh in Global.Meshes.world_transparent)
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
            foreach (Mesh mesh in Global.Meshes.gizmos)
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
    }


}
