using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MyFirstWPF.Consts;
using MyFirstWPF.Models;

namespace MyFirstWPF.Services
{
    public class NodeService
    {
        public static Ellipse GetEllipse(double radius, Brush ellipseColor, Brush borderColor)
        {
            return new Ellipse()
            {
                Height = radius,
                Width = radius,
                Fill = ellipseColor,
                Stroke = borderColor

            };
        }

        public static Tuple<Point, Point> ReduceArrowLine(Point point1, Point point2)
        {
            var point3 = new Point();
            var newPoint1 = new Point();
            var newPoint2 =new Point();
           
            var sin1 = 0.0;
            var sin2 = 0.0;
            var cos1 = 0.0;
            var cos2 = 0.0;
            var arrowLineLenght = VectorLen(point1, point2);


            point3.X = Math.Min(point1.X, point2.X);

            if (point1.X < point2.X && point1.Y > point2.Y)
            {
                point3.Y = point2.Y;
                newPoint1 = new Point(point1.X, point1.Y - StartParameters.NodeRadius);
                newPoint2 = new Point(point2.X - StartParameters.NodeRadius, point2.Y);

                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = -VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;

            }
            if (point2.X < point1.X && point1.Y < point2.Y)
            {
                point3.Y = point1.Y;
                newPoint1 = new Point(point1.X - StartParameters.NodeRadius, point1.Y);
                newPoint2 = new Point(point2.X, point2.Y - StartParameters.NodeRadius);

                sin1 = -VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;
            }

            if (point1.X < point2.X && point1.Y < point2.Y)
            {
                point3.Y = point2.Y;
                newPoint1 = new Point(point1.X, point1.Y + StartParameters.NodeRadius);
                newPoint2 = new Point(point2.X - StartParameters.NodeRadius, point2.Y);



                sin1 = -VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;
            }

            if (point1.X > point2.X && point1.Y > point2.Y)
            {
                point3.Y = point1.Y;
                newPoint1 = new Point(point1.X - StartParameters.NodeRadius, point1.Y);
                newPoint2 = new Point(point2.X, point2.Y + StartParameters.NodeRadius);


                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = -VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;
            }

            newPoint1 = RotatePoint(newPoint1, point1, cos1, sin1);
            newPoint2 = RotatePoint(newPoint2, point2, cos2, sin2);

            if (point1.X == point2.X)
            {
                if (point1.Y > point2.Y)
                {
                    newPoint1 = new Point(point1.X, point1.Y - StartParameters.NodeRadius);
                    newPoint2 = new Point(point2.X, point2.Y + StartParameters.NodeRadius);
                }
                else
                {
                    newPoint1 = new Point(point1.X, point1.Y + StartParameters.NodeRadius);
                    newPoint2 = new Point(point2.X, point2.Y - StartParameters.NodeRadius);
                }
            }

            if (point1.Y == point2.Y)
            {
                if (point1.X > point2.X)
                {
                    newPoint1 = new Point(point1.X - StartParameters.NodeRadius, point1.Y);
                    newPoint2 = new Point(point2.X + StartParameters.NodeRadius, point2.Y);
                }
                else
                {
                    newPoint1 = new Point(point1.X + StartParameters.NodeRadius, point1.Y);
                    newPoint2 = new Point(point2.X - StartParameters.NodeRadius, point2.Y);
                }
            }

            return new Tuple<Point, Point>(newPoint1, newPoint2);
        }

        public static double VectorLen(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.X - point1.X, 2));
        }

        private static Point RotatePoint(Point src, Point rel, double cos, double sin)
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

        /// <summary>
        /// Установка цета узла
        /// </summary>
        public static void SetNodeVmColor(ref NodeVm nodeVm, bool nullFlag = false, Brush border = null)
        {
            if (nodeVm == null) return;

            nodeVm.TextBlock.Background = new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.NormalBackground, border ??  NodeColors.NormalBorder));
            
            if (nodeVm.Node.IsStartNode)
            {
                nodeVm.TextBlock.Background =
               new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.StartNodeBackground, border ?? NodeColors.NormalBorder));
            }

            if (nodeVm.Node.IsRejectionNode)
            {
                nodeVm.TextBlock.Background =
               new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.RejectionNodeBackground, border ?? NodeColors.NormalBorder));
            }

            if (nullFlag)
                nodeVm = null;
        }

        public static void SetEdgeVmColor(ref EdgeVm edgeVm, bool nullFlag = false)
        {
            if (edgeVm == null) return;

            edgeVm.ArrowLine.Stroke = EdgeColors.NormalEdge;

            if (nullFlag)
                edgeVm = null;
        }
    }
}
