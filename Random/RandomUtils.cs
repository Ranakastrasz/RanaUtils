using System;
using System.Drawing;
using System.Numerics;

using RanaUtils.Mathematics;

using RandomExtensions;

namespace RanaUtils.Random
{
    public static class RandomUtils
    {
        private static System.Random Instance = new();
        static float Value => Instance.NextFloat();

        public static void InitSeed(int seed)
        {
            RandomUtils.Instance = new(seed);
        }

        public static float Range(float minInclusive, float maxInclusive)
        { 
            return (float)(RandomUtils.Instance.NextDouble() * (maxInclusive - minInclusive) + minInclusive);
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

        static Vector2 InsideRect(RectangleF rect)
        {
            float x = Range(rect.Left, rect.Right);
            float y = Range(rect.Top, rect.Bottom);
            return new Vector2(x, y);
        }

    }
}
