using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace RayTwol
{
    public struct Colour
    {
        public byte r;
        public byte g;
        public byte b;

        public Colour(byte r = 0, byte g = 0, byte b = 0)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        static Random rnd = new Random();
        public static Colour RandomColour()
        {
            byte R = (byte)rnd.Next(40, 255);
            byte G = (byte)rnd.Next(40, 255);
            byte B = (byte)rnd.Next(40, 255);
            return new Colour(R, G, B);
        }

        public static Colour RandomColourSeeded(int seed)
        {
            rnd = new Random(seed);
            byte R = (byte)rnd.Next(40, 255);
            byte G = (byte)rnd.Next(40, 255);
            byte B = (byte)rnd.Next(40, 255);
            return new Colour(R, G, B);
        }
    }


    


    partial class MainWindow
    {
        public Camera cam = new Camera();


        public int width;
        public int height;
    }


}
