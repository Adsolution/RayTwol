using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTwol
{
    /// <summary>
    /// Represents a three-dimensional coordinate.
    /// </summary>
    public struct Vec3
    {
        public static bool operator ==(Vec3 a, Vec3 b)
        {
            if (a.x == b.x && a.y == b.y && a.z == b.z)
                return true;
            else
                return false;
        }
        public static bool operator !=(Vec3 a, Vec3 b)
        {
            if (a.x == b.x && a.y == b.y && a.z == b.z)
                return false;
            else
                return true;
        }
        public static Vec3 operator *(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vec3 operator *(Vec3 a, float b)
        {
            return new Vec3(a.x * b, a.y * b, a.z * b);
        }
        public static Vec3 operator *(float a, Vec3 b)
        {
            return new Vec3(a * b.x, a * b.y, a * b.z);
        }
        public static Vec3 operator /(Vec3 a, float b)
        {
            return new Vec3(a.x / b, a.y / b, a.z / b);
        }
        public static Vec3 operator /(float a, Vec3 b)
        {
            return new Vec3(a / b.x, a / b.y, a / b.z);
        }
        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vec3 operator -(Vec3 a, Vec3 b)
        {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public float x, y, z;
        public bool isValid;
        public Vec3(float x = 0, float y = 0, float z = 0, bool isValid = true)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.isValid = isValid;
        }

        public static float dist(Vec3 a, Vec3 b)
        {
            float b2 = (float)Math.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
            return (float)Math.Sqrt(b2 * b2 + (a.z - b.z) * (a.z - b.z));
        }
    }

    public class Matrix
    {
        public static float[] Identity = new float[16]
                    {
                        1, 0, 0, 0,
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1
                    };

        public static float[] Multiply(float[] a, float[] b)
        {
            float[] ret = Identity;

            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 4; x++)
                {
                    float temp = 0;
                    for (int i = 0; i < 4; i++)
                        temp += a[i + y * 4] * b[x + i * 4];

                    ret[x + y * 4] = temp;
                }

            return ret;
        }
    }




    public class Transform
    {
        public Transform()
        {
            scale = new Vec3(1, 1, 1);
        }

        public float[] matrix = Matrix.Identity;

        Vec3 _pos;
        public Vec3 pos
        {
            get
            {
                return _pos;
            }
            set
            {
                if (pos != pos)
                {
                    float[] m = new float[16]
                    {
                        1,       0,       0,       pos.x,
                        0,       1,       0,       pos.y,
                        0,       0,       1,       pos.z,
                        0,       0,       0,       1
                    };
                    matrix = Matrix.Multiply(matrix, m);
                }
                _pos = value;
            }
        }

        Vec3 _rot;
        public Vec3 rot
        {
            get
            {
                return _rot;
            }
            set
            {
                if (rot.x != rot.x)
                {
                    float cos = Direction.AngleToDir(rot.x).cos;
                    float sin = Direction.AngleToDir(rot.x).sin;
                    float[] m = new float[16]
                    {
                        1,       0,       0,       0,
                        0,       cos,     -sin,    0,
                        0,       sin,     cos,     0,
                        0,       0,       0,       1
                    };
                    matrix = Matrix.Multiply(matrix, m);
                }
                if (rot.y != rot.y)
                {
                    float cos = Direction.AngleToDir(rot.y).cos;
                    float sin = Direction.AngleToDir(rot.y).sin;
                    float[] m = new float[16]
                    {
                        cos,     0,       sin,     0,
                        0,       0,       0,       0,
                        -sin,    0,       cos,     0,
                        0,       0,       0,       1
                    };
                    matrix = Matrix.Multiply(matrix, m);
                }
                if (rot.z != rot.z)
                {
                    float cos = Direction.AngleToDir(rot.z).cos;
                    float sin = Direction.AngleToDir(rot.z).sin;
                    float[] m = new float[16]
                    {
                        cos,     -sin,    0,      0,
                        sin,     cos,     0,      0,
                        0,       0,       1,      0,
                        0,       0,       0,      1
                    };
                    matrix = Matrix.Multiply(matrix, m);
                }
                _rot = value;
            }
        }

        Vec3 _scale;
        public Vec3 scale
        {
            get
            {
                return _scale;
            }
            set
            {
                if (scale != scale)
                {
                    float[] m = new float[16]
                    {
                        scale.x, 0,       0,       0,
                        0,       scale.y, 0,       0,
                        0,       0,       scale.z, 0,
                        0,       0,       0,       1
                    };
                    matrix = Matrix.Multiply(matrix, m);
                }
                _scale = value;
            }
        }
    }


    /// <summary>
    /// Represents a two-dimensional coordinate.
    /// </summary>
    public struct Vec2
    {
        public static Vec2 operator *(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x * b.x, a.y * b.y);
        }
        public static Vec2 operator *(Vec2 a, float b)
        {
            return new Vec2(a.x * b, a.y * b);
        }
        public static Vec2 operator *(float a, Vec2 b)
        {
            return new Vec2(a * b.x, a * b.y);
        }
        public static Vec2 operator /(Vec2 a, float b)
        {
            return new Vec2(a.x / b, a.y / b);
        }
        public static Vec2 operator /(float a, Vec2 b)
        {
            return new Vec2(a / b.x, a / b.y);
        }
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public float x, y;
        public Vec2(float x = 0, float y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public static float GetDistance(Vec2 a, Vec2 b)
        {
            return (float)Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
        }
    }

    
  


    public struct Direction
    {
        public float cos;
        public float sin;

        public Direction(float cos, float sin)
        {
            this.cos = cos;
            this.sin = sin;
        }

        public static Direction AngleToDir(float angle)
        {
            return new Direction
            (
                (float)Math.Cos(Math.PI * (angle / 180)),
                (float)Math.Sin(Math.PI * (angle / 180))
            );
        }


        
        public static Vec3 Forward(Vec3 rotation)
        {
            return new Vec3
            (
                -AngleToDir(rotation.y).sin * AngleToDir(rotation.x).cos,
                AngleToDir(rotation.x).sin,
                AngleToDir(rotation.y).cos * AngleToDir(rotation.x).cos
            );
        }
        public static Vec3 Backward(Vec3 rotation)
        {
            return new Vec3
            (
                -AngleToDir(rotation.y + 180).sin * AngleToDir(rotation.x).cos,
                -AngleToDir(rotation.x).sin,
                AngleToDir(rotation.y).cos * AngleToDir(rotation.x + 180).cos
            );
        }
        public static Vec3 Left(Vec3 rotation) // Works for now --- does not support rolling
        {
            rotation.y -= 90;
            return new Vec3
            (
                -AngleToDir(rotation.y).sin,
                0,
                AngleToDir(rotation.y).cos
            );
        }
        public static Vec3 Right(Vec3 rotation) // Works for now --- does not support rolling
        {
            rotation.y += 90;
            return new Vec3
            (
                -AngleToDir(rotation.y).sin,
                0,
                AngleToDir(rotation.y).cos
            );
        }
        public static Vec3 Up(Vec3 rotation) // Untested
        {
            rotation.x += 90;
            return new Vec3
            (
                -AngleToDir(rotation.y).sin * AngleToDir(rotation.x).cos,
                AngleToDir(rotation.x).sin,
                AngleToDir(rotation.y).cos * AngleToDir(rotation.x).cos
            );
        }
        public static Vec3 Down(Vec3 rotation) // Untested
        {
            rotation.x -= 90;
            return new Vec3
            (
                AngleToDir(rotation.y).sin * AngleToDir(rotation.x).cos,
                AngleToDir(rotation.x).sin,
                AngleToDir(rotation.y).cos * AngleToDir(rotation.x).cos
            );
        }
    }
}
