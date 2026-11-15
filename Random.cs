using System;
using System.Drawing;
using System.Numerics;

using RanaUtils.Mathematics;

using RandomExtensions;

namespace RanaUtils
{
    public static class Random
    {
        private static System.Random s_instance = new();
        static float Value => s_instance.NextFloat();

        public static void InitSeed(int seed)
        {
            s_instance = new(seed);
        }

        public static float Range(float minInclusive, float maxInclusive)
        { 
            return (float)(s_instance.NextDouble() * (maxInclusive - minInclusive) + minInclusive);
        }

        //public static Color ColorHSV()
        //{
        //    float hue = Value;
        //    float saturation = Value;
        //    float value = Value;
        //    return Color.FromHSV(hue, saturation, value);
        //}

        static Vector2 InsideCircle(Circle circle)
        {
            float radius = circle.Radius * (float)Math.Sqrt(Value);
            float theta = Value * 2 * (float)Math.PI;
            float x = (float)(radius * Math.Cos(theta));
            float y = (float)(radius * Math.Sin(theta));
            return new Vector2(x, y);
        }

        static Vector2 InsideRect(RectangleF rect) // I suspect I will want to make my own Rectangle object instead.
        {
            float x = Range(rect.Left, rect.Right);
            float y = Range(rect.Top, rect.Bottom);
            return new Vector2(x, y);
        }

    }
}
