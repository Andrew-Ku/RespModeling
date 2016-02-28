using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
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
    }
}
