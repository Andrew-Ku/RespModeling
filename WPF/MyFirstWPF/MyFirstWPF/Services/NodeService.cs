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

        public static Tuple<Point,Point> ReduceArrowLine(Point point1, Point point2)
        {
            var subLenght = StartParameters.NodeRadius;

            var point3 = new Point();
            var newPoint1 = new Point();
            var newPoint2 = new Point();

            var arrowLineLenght = Math.Sqrt(Math.Pow(point2.Y - point1.Y, 2) + Math.Pow(point2.X - point1.X, 2));

            newPoint1.X = subLenght * ((point2.X - point1.X) / arrowLineLenght);
            newPoint1.Y = subLenght * ((point2.Y - point1.Y) / arrowLineLenght);




            if (point1.X == point2.X)
            {
                newPoint1.X = point1.X;
                if (point1.Y > point2.Y)
                {
                    newPoint1.Y = point1.Y - subLenght;
                }
                if (point1.Y <= point2.Y)
                {
                    newPoint1.Y = point1.Y + subLenght;
                }
                
            }
            else if (point1.Y == point2.Y)
            {
                newPoint1.Y = point1.Y;
                if (point1.X > point2.X)
                {
                    newPoint1.X = point1.X - subLenght;
                }
                if (point1.X <= point2.X)
                {
                    newPoint1.X = point1.X + subLenght;
                }

            }

            else if (point1.X < point2.X && point1.Y < point2.Y)
            {
                newPoint1.X = point1.X + newPoint1.X;
                newPoint1.Y = point1.Y - newPoint1.Y;
            }
            //else if (point1.X < point2.X && point1.Y > point2.Y)
            //{
            //    newPoint1.X = point1.X - newPoint1.X;
            //    newPoint1.Y = point1.Y + newPoint1.Y;
            //}
            
            else
            {
                newPoint1.X = point1.X + newPoint1.X;
                newPoint1.Y = point1.Y + newPoint1.Y;
            }



            return new Tuple<Point, Point>(point1,point1);
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
    }
}
