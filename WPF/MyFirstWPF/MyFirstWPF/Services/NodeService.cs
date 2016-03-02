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
        public Ellipse GetEllipse(double radius, Brush ellipseColor, Brush borderColor)
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
            var subLenght = StartParameters.NodeRadius;

            var point3 = new Point();
            var newPoint1 = new Point();
            var newPoint2 = new Point();

            int signX1 = 0;
            int signY1 = 0;
            int signX2 = 0;
            int signY2 = 0;
            double sin1 = 0.0;
            double sin2 = 0.0;
            double cos1 = 0.0;
            double cos2 = 0.0;
            var arrowLineLenght = VectorLen(point1, point2);


            point3.X = Math.Min(point1.X, point2.X);

            if (point1.X < point2.X && point1.Y > point2.Y)
            {
                point3.Y = point2.Y;
                signX1 = 1;
                signY1 = -1;

                signX2 = (-1) * signX1;
                signY2 = (-1) * signY1;

                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;

                var arcSin1 = Math.Asin(sin1);
                var arcSin2 = Math.Asin(sin2);
                var arcCos1 = Math.Asin(cos1);
                var arcCos2 = Math.Asin(cos2);
            }

            if (point2.X < point1.X && point1.Y < point2.Y)
            {
                point3.Y = point1.Y;
                signX1 = -1;
                signY1 = +1;

                signX2 = (-1) * signX1;
                signY2 = (-1) * signY1;

                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;

                var arcSin1 = Math.Asin(sin1);
                var arcSin2 = Math.Asin(sin2);
                var arcCos1 = Math.Asin(cos1);
                var arcCos2 = Math.Asin(cos2);
            }

            if (point1.X < point2.X && point1.Y < point2.Y)
            {
                point3.Y = point2.Y;
                signX1 = 1;
                signY1 = 1;

                signX2 = (-1) * signX1;
                signY2 = (-1) * signY1;

                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;

                var arcSin1 = Math.Asin(sin1);
                var arcSin2 = Math.Asin(sin2);
                var arcCos1 = Math.Asin(cos1);
                var arcCos2 = Math.Asin(cos2);
            }

            if (point1.X > point2.X && point1.Y > point2.Y)
            {
                point3.Y = point1.Y;
                signX1 = -1;
                signY1 = -1;

                signX2 = (-1) * signX1;
                signY2 = (-1) * signY1;

                sin1 = VectorLen(point2, point3) / arrowLineLenght;
                cos1 = VectorLen(point1, point3) / arrowLineLenght;

                sin2 = VectorLen(point1, point3) / arrowLineLenght;
                cos2 = VectorLen(point2, point3) / arrowLineLenght;

                var arcSin1 = Math.Asin(sin1);
                var arcSin2 = Math.Asin(sin2);
                var arcCos1 = Math.Asin(cos1);
                var arcCos2 = Math.Asin(cos2);
            }



            var x1Offset = subLenght * sin1 * signX1;
            var y1Offset = subLenght * cos1 * signY1;

            var x2Offset = subLenght * sin2 * signX2;
            var y2Offset = subLenght * cos2 * signY2;

            newPoint1.X = point1.X + x1Offset;
            newPoint1.Y = point1.Y + y1Offset;

            newPoint2.X = point2.X + x2Offset;
            newPoint2.Y = point2.Y + y2Offset;


            return new Tuple<Point, Point>(newPoint1, newPoint2);
        }

        private static double VectorLen(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.X - point1.X, 2));
        }



        //public static ArrowLine ReduceArrowLine(ArrowLine arrowLine, double subLenght)
        //{
        //    //var point1 = new Point(arrowLine.X1, arrowLine.Y1);
        //    //var point2 = new Point(arrowLine.X2, arrowLine.Y2);

        //    //var newPoint1 = new Point();
        //    //var newPoint2 = new Point();

        //    //var arrowLineLenght = Math.Sqrt(Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.X - point1.X, 2));

        //    //newPoint1.X = subLenght*(Math.Abs(point2.Y - point1.Y)/arrowLineLenght);
        //    //newPoint1.Y = subLenght*(Math.Abs(point2.X - point1.X)/arrowLineLenght);

        //    //newPoint2.X = subLenght*(Math.Abs(point2.X - point1.X)/arrowLineLenght);
        //    //newPoint2.Y = subLenght*Math.Cos(Math.Abs(point2.Y - point1.Y)/arrowLineLenght);  

        //    //newPoint2.X = subLenght*((point2.X - point1.X)/arrowLineLenght);
        //    //newPoint2.Y = subLenght*Math.Cos(.Abs(point2.Y - point1.Y)/arrowLineLenght);


        //    //if (point1.X > point2.X)
        //    //{
        //    //    newPoint1.X = point1.X - newPoint1.X;
        //    //    newPoint2.X = point2.X + newPoint2.X;

        //    //}
        //    //else
        //    //{
        //    //    newPoint1.X = point1.X + newPoint1.X;
        //    //    newPoint2.X = point2.X - newPoint2.X;
        //    //}

        //    //if (point1.Y > point2.Y)
        //    //{
        //    //    newPoint1.Y = point1.Y - newPoint1.Y;
        //    //    newPoint2.Y = point2.Y + newPoint2.Y;

        //    //}
        //    //else
        //    //{
        //    //    newPoint1.Y = point1.Y + newPoint1.Y;
        //    //    newPoint2.Y = point2.Y - newPoint2.Y;
        //    //}


        //    //arrowLine.X1 = newPoint1.X;
        //    //arrowLine.X2 = newPoint2.X;

        //    //arrowLine.Y1 = newPoint1.Y;
        //    //arrowLine.Y2 = newPoint2.Y;

        //    arrowLine.X1 += 5;
        //    arrowLine.X2 += 5;

        //    arrowLine.Y1 += 5;
        //    arrowLine.Y2 += 5;


        //    return arrowLine;
        //}


        private Point? Rotate(Point src, Point rel, double cos, double sin)
        {
            var srcMat = new[] {src.X, src.Y,1.0};
            var transfetMat = new[,] {{1.0, 0.0, 0.0}, {0.0, 1.0, 9.2}, {-rel.X, -rel.Y, 1.0}};
            var rotateMat = new[,] { { cos, sin, 0.0 }, { -sin, cos, 0.0 }, { 0.0, 0.1, 1.0 } };
            var transfetMatObr = new[,] { { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 9.2 }, { rel.X, rel.Y, 1.0 } };



            var m = new Matrix();
           //m.RotateAt();



            var res = MultipleMatrix(MultipleMatrix(MultipleVectorMatrix(srcMat,transfetMat),rotateMat),transfetMatObr);

          
            return null;

        }

        private double[,] MultipleMatrix(double[,] leftM, double[,] rightM)
        {
            var r = new double[leftM.GetLength(0), rightM.GetLength(1)];
            for (var i = 0; i < leftM.GetLength(0); i++)
            {
                for (var j = 0; j < rightM.GetLength(1); j++)
                {
                    for (var k = 0; k < rightM.GetLength(0); k++)
                    {
                        r[i, j] += leftM[i, k] * rightM[k, j];
                    }
                }
            }
            return r;

        }

        private double[,] MultipleVectorMatrix(double[] leftM, double[,] rightM)
        {
            var r = new double[leftM.GetLength(0), rightM.GetLength(1)];
            for (var i = 0; i < leftM.GetLength(0); i++)
            {
                for (var j = 0; j < rightM.GetLength(1); j++)
                {
                    for (var k = 0; k < rightM.GetLength(0); k++)
                    {
                        r[i, j] += leftM[i] * rightM[k, j];
                    }
                }
            }
            return r;

        }


    }




}
