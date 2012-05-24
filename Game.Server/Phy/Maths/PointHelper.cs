using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Phy.Maths
{
    public class PointHelper
    {
        public static Point Normalize(Point point,int len)
        {
            double l = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            return new Point((int)(point.X *  len / l ), (int)(point.Y * len / l));
        }
    }
}
