using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K = System.Windows.Forms.Keys;
using M = System.Windows.Forms.MouseButtons;

namespace RayTwol
{
    partial class MainWindow
    {
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
    }
}
