using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MyFirstWPF.Consts;
using MyFirstWPF.Services;

namespace MyFirstWPF.Models
{
    public class ArrowLine : Shape
    {
        protected PathGeometry pathgeo;
        protected PathFigure pathfigLine;
        protected PolyLineSegment polysegLine;

        PathFigure pathfigHead1;
        PolyLineSegment polysegHead1;
        PathFigure pathfigHead2;
        PolyLineSegment polysegHead2;

        public ArrowLine()
        {
            pathgeo = new PathGeometry();

            pathfigLine = new PathFigure();
            polysegLine = new PolyLineSegment();
            pathfigLine.Segments.Add(polysegLine);

            pathfigHead1 = new PathFigure();
            polysegHead1 = new PolyLineSegment();
            pathfigHead1.Segments.Add(polysegHead1);

            pathfigHead2 = new PathFigure();
            polysegHead2 = new PolyLineSegment();
            pathfigHead2.Segments.Add(polysegHead2);

        }

        public ArrowLine(Point point1, Point point2)
        {
            pathgeo = new PathGeometry();

            pathfigLine = new PathFigure();
            polysegLine = new PolyLineSegment();
            pathfigLine.Segments.Add(polysegLine);

            pathfigHead1 = new PathFigure();
            polysegHead1 = new PolyLineSegment();
            pathfigHead1.Segments.Add(polysegHead1);

            pathfigHead2 = new PathFigure();
            polysegHead2 = new PolyLineSegment();
            pathfigHead2.Segments.Add(polysegHead2);
            PositionUpdate(point1, point2);

        }

        /// <summary>
        ///     Identifies the ArrowEnds dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowEndsProperty =
            DependencyProperty.Register("ArrowEnds",
                typeof(ArrowEnds), typeof(ArrowLine),
                new FrameworkPropertyMetadata(ArrowEnds.End,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the property that determines which ends of the
        ///     line have arrows.
        /// </summary>
        public ArrowEnds ArrowEnds
        {
            set { SetValue(ArrowEndsProperty, value); }
            get { return (ArrowEnds)GetValue(ArrowEndsProperty); }
        }

        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double X1
        {
            set { SetValue(X1Property, value); }
            get { return (double)GetValue(X1Property); }
        }


        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Y1
        {
            set { SetValue(Y1Property, value); }
            get { return (double)GetValue(Y1Property); }
        }

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double X2
        {
            set { SetValue(X2Property, value); }
            get { return (double)GetValue(X2Property); }
        }

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(0.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double Y2
        {
            set { SetValue(Y2Property, value); }
            get { return (double)GetValue(Y2Property); }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Clear out the PathGeometry.
                pathgeo.Figures.Clear();

                // Define a single PathFigure with the points.
                pathfigLine.StartPoint = new Point(X1, Y1);
                polysegLine.Points.Clear();
                polysegLine.Points.Add(new Point(X2, Y2));
                pathgeo.Figures.Add(pathfigLine);

                // Call the base property to add arrows on the ends.
                int count = polysegLine.Points.Count;

                if (count > 0)
                {
                    // Draw the arrow at the start of the line.
                    if ((ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
                    {
                        Point pt1 = pathfigLine.StartPoint;
                        Point pt2 = polysegLine.Points[0];
                        pathgeo.Figures.Add(CalculateArrow(pathfigHead1, pt2, pt1));
                    }

                    // Draw the arrow at the end of the line.
                    if ((ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
                    {
                        Point pt1 = count == 1 ? pathfigLine.StartPoint :
                                                 polysegLine.Points[count - 2];
                        Point pt2 = polysegLine.Points[count - 1];
                        pathgeo.Figures.Add(CalculateArrow(pathfigHead2, pt1, pt2));
                    }
                }
                return pathgeo;
            }
        }
        /// <summary>
        ///     Identifies the ArrowAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowAngleProperty =
            DependencyProperty.Register("ArrowAngle",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(45.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the angle between the two sides of the arrowhead.
        /// </summary>
        public double ArrowAngle
        {
            set { SetValue(ArrowAngleProperty, value); }
            get { return (double)GetValue(ArrowAngleProperty); }
        }

        /// </summary>
        public static readonly DependencyProperty ArrowLengthProperty =
            DependencyProperty.Register("ArrowLength",
                typeof(double), typeof(ArrowLine),
                new FrameworkPropertyMetadata(12.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the length of the two sides of the arrowhead.
        /// </summary>
        public double ArrowLength
        {
            set { SetValue(ArrowLengthProperty, value); }
            get { return (double)GetValue(ArrowLengthProperty); }
        }

        /// <summary>
        ///     Identifies the IsArrowClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsArrowClosedProperty =
            DependencyProperty.Register("IsArrowClosed",
                typeof(bool), typeof(ArrowLine),
                new FrameworkPropertyMetadata(false,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the property that determines if the arrow head
        ///     is closed to resemble a triangle.
        /// </summary>
        public bool IsArrowClosed
        {
            set { SetValue(IsArrowClosedProperty, value); }
            get { return (bool)GetValue(IsArrowClosedProperty); }
        }

        PathFigure CalculateArrow(PathFigure pathfig, Point pt1, Point pt2)
        {
            var matx = new Matrix();
            var vect = pt1 - pt2;
            vect.Normalize();
            vect *= ArrowLength;

            var polyseg = pathfig.Segments[0] as PolyLineSegment;
            polyseg.Points.Clear();
            matx.Rotate(ArrowAngle / 2);
            pathfig.StartPoint = pt2 + vect * matx;
            polyseg.Points.Add(pt2);

            matx.Rotate(-ArrowAngle);
            polyseg.Points.Add(pt2 + vect * matx);
            pathfig.IsClosed = IsArrowClosed;

            return pathfig;
        }

        public void PositionUpdate(Point point1, Point point2)
        {
            var point3 = new Point();
            var newPoint1 = new Point();
            var newPoint2 = new Point();

            var sin1 = 0.0;
            var sin2 = 0.0;
            var cos1 = 0.0;
            var cos2 = 0.0;
            var arrowLineLenght = MathService.VectorLen(point1, point2);

            point3.X = Math.Min(point1.X, point2.X);

            if (point1.X < point2.X && point1.Y > point2.Y)
            {
                point3.Y = point2.Y;
                newPoint1 = new Point(point1.X, point1.Y - StartParameters.NodeRadius);
                newPoint2 = new Point(point2.X - StartParameters.NodeRadius, point2.Y);

                sin1 = MathService.VectorLen(point2, point3) / arrowLineLenght;
                cos1 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                sin2 = -MathService.VectorLen(point1, point3) / arrowLineLenght;
                cos2 = MathService.VectorLen(point2, point3) / arrowLineLenght;

            }
            if (point2.X < point1.X && point1.Y < point2.Y)
            {
                point3.Y = point1.Y;
                newPoint1 = new Point(point1.X - StartParameters.NodeRadius, point1.Y);
                newPoint2 = new Point(point2.X, point2.Y - StartParameters.NodeRadius);

                sin1 = -MathService.VectorLen(point2, point3) / arrowLineLenght;
                cos1 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                sin2 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                cos2 = MathService.VectorLen(point2, point3) / arrowLineLenght;
            }

            if (point1.X < point2.X && point1.Y < point2.Y)
            {
                point3.Y = point2.Y;
                newPoint1 = new Point(point1.X, point1.Y + StartParameters.NodeRadius);
                newPoint2 = new Point(point2.X - StartParameters.NodeRadius, point2.Y);

                sin1 = -MathService.VectorLen(point2, point3) / arrowLineLenght;
                cos1 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                sin2 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                cos2 = MathService.VectorLen(point2, point3) / arrowLineLenght;
            }

            if (point1.X > point2.X && point1.Y > point2.Y)
            {
                point3.Y = point1.Y;
                newPoint1 = new Point(point1.X - StartParameters.NodeRadius, point1.Y);
                newPoint2 = new Point(point2.X, point2.Y + StartParameters.NodeRadius);

                sin1 = MathService.VectorLen(point2, point3) / arrowLineLenght;
                cos1 = MathService.VectorLen(point1, point3) / arrowLineLenght;
                sin2 = -MathService.VectorLen(point1, point3) / arrowLineLenght;
                cos2 = MathService.VectorLen(point2, point3) / arrowLineLenght;
            }

            newPoint1 = MathService.RotatePoint(newPoint1, point1, cos1, sin1);
            newPoint2 = MathService.RotatePoint(newPoint2, point2, cos2, sin2);

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

            X1 = newPoint1.X;
            Y1 = newPoint1.Y;
            X2 = newPoint2.X;
            Y2 = newPoint2.Y;
        }
    }
}
