using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RayTwol
{
    partial class MainWindow
    {
        Vec3 freezePos = new Vec3();

        void GameSync()
        {
            button_Run.Content = Memory.runButtonText;

            if (Memory.isSynced)
            {
                viewport.Margin = new Thickness(273, 41, 234, 41);

                Memory.rayPos = Memory.GetRaymanPosition();

                textbox_RayPosX.Text = Memory.rayPos.x.ToString("0.00");
                textbox_RayPosY.Text = Memory.rayPos.y.ToString("0.00");
                textbox_RayPosZ.Text = Memory.rayPos.z.ToString("0.00");

                if ((bool)checkbox_RayFreeze.IsChecked)
                    Memory.SetRaymanPosition(freezePos);
            }

            else
            {
                viewport.Margin = new Thickness(273, 41, 5, 41);
            }
        }

        void checkbox_RayFreeze_Checked(object sender, RoutedEventArgs e)
        {
            freezePos = Memory.rayPos;
        }
        
        void button_RayWarpToCamera_Click(object sender, RoutedEventArgs e)
        {
            Vec3 camPosAdjusted = new RayTwol.Vec3(cam.pos.x, -cam.pos.z, -cam.pos.y);
            freezePos = camPosAdjusted;
            Memory.SetRaymanPosition(camPosAdjusted);
        }
    }
}
