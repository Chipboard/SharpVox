using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics.Glsl;

namespace SharpVox.Utilities
{
    class VectorMath
    {
        public static Vec3 Multiply(Vec3 one, float two)
        {
            return new Vec3(one.X * two, one.Y * two, one.Z * two);
        }

        public static Vec2 Multiply(Vec2 one, float two)
        {
            return new Vec2(one.X * two, one.Y * two);
        }

        public static Vec3 Multiply(Vec3 one, Vec3 two)
        {
            return new Vec3(one.X * two.X, one.Y * two.Y, one.Z * two.Z);
        }

        public static Vec2 Multiply(Vec2 one, Vec2 two)
        {
            return new Vec2(one.X * two.X, one.Y * two.Y);
        }

        public static Vec3 Add(Vec3 one, Vec3 two)
        {
            return new Vec3(one.X + two.X, one.Y + two.Y, one.Z + two.Z);
        }

        public static Vec2 Add(Vec2 one, Vec2 two)
        {
            return new Vec2(one.X + two.X, one.Y + two.Y);
        }

        public static Vec3 Subtract(Vec3 one, Vec3 two)
        {
            return new Vec3(one.X - two.X, one.Y - two.Y, one.Z - two.Z);
        }

        public static Vec2 Subtract(Vec2 one, Vec2 two)
        {
            return new Vec2(one.X - two.X, one.Y - two.Y);
        }

        public static Vec3 Normalize(Vec3 vec)
        {
            float distance = (float)Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z));

            if (distance == 0)
                return vec;
            else
                return new Vec3(vec.X / distance, vec.Y / distance, vec.Z / distance);
        }

        public static Vec2 Normalize(Vec2 vec)
        {
            float distance = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);

            if (distance == 0)
                return vec;
            else
                return new Vec2(vec.X / distance, vec.Y / distance);
        }

        public static Vec3 Invert(Vec3 vec)
        {
            return new Vec3(-vec.X, -vec.Y, -vec.Z);
        }

        public static Vec2 Invert(Vec2 vec)
        {
            return new Vec2(-vec.X, -vec.Y);
        }

        public static Vec3 Cross(Vec3 one, Vec3 two)
        {
            float x = one.Y * two.Z - two.Y * one.Z;
            float y = (one.X * two.Z - two.X * one.Z) * -1;
            float z = one.X * two.Y - two.X * one.Y;

            return new Vec3(x, y, z);
        }
    }
}
