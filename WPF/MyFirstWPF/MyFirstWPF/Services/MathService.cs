using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyFirstWPF.Services
{
    public static class MathService
    {
        public static double VectorLen(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.X - point1.X, 2));
        }

        public static Point RotatePoint(Point src, Point rel, double cos, double sin)
        {
            var srcMat = new[] { src.X, src.Y, 1.0 };
            var transfetMat = new[,] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { -rel.X, -rel.Y, 1.0 } };
            var rotateMat = new[,] { { cos, sin, 0.0 }, { -sin, cos, 0.0 }, { 0.0, 0.1, 1.0 } };
            var transfetMatObr = new[,] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 }, { rel.X, rel.Y, 1.0 } };

            var res = MultipleVectorMatrix(MultipleVectorMatrix(MultipleVectorMatrix(srcMat, transfetMat), rotateMat), transfetMatObr);

            return new Point(res[0], res[1]);
        }

        private static double[] MultipleVectorMatrix(double[] leftM, double[,] rightM)
        {
            if (leftM.Length != rightM.GetLength(0))
            {
                throw new ArgumentException();
            }

            var r = new double[leftM.Length];

            for (var j = 0; j < leftM.GetLength(0); j++)
            {
                r[0] += leftM[j] * rightM[j, 0];
                r[1] += leftM[j] * rightM[j, 1];
                r[2] += leftM[j] * rightM[j, 2];
            }

            return r;
        }
    }
}
