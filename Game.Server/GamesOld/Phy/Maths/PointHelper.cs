using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Phy.Maths
{
    public static class PointHelper
    {
        public static void Normalize(this Point point,int len)
        {
            double l = point.Length();
            point.X = (int)(point.X *  len / l );
            point.Y = (int)(point.Y *  len / l  );
        }

        public static double Length(this Point point)
        {
            return Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        public static double Distance(this Point point, Point target)
        {
            int dx = point.X - target.X;
            int dy = point.Y - target.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double Distance(this Point point, int tx, int ty)
        {
            int dx = point.X - tx;
            int dy = point.Y - ty;

            return Math.Sqrt(dx * dx + dy * dy);
        }


        public static void Normalize(this PointF point, float len)
        {
            double l = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            point.X = (float)(point.X * len / l);
            point.Y = (float)(point.Y * len / l);
        }

        public static double Length(this PointF point)
        {
            return Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }
    }
}
