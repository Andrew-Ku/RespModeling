using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace MyFirstWPF.Models
{
    public class NodeVm : DependencyObject
    {
        public static int NodeVmCount;

        public NodeVm()
        {
            EdgeVmList = new List<EdgeVm>();
        }

        [JsonIgnore] 
        public TextBlock TextBlock { get; set; }

        public Node Node { get; set; }

        public static readonly DependencyProperty PositionProperty =DependencyProperty.Register("Position",typeof(Point), typeof(NodeVm),
            new FrameworkPropertyMetadata(new Point(),FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Point Position
        {
            set
            {
                SetValue(PositionProperty, value);
                foreach (var edgeVm in EdgeVmList)
                {
                    edgeVm.ArrowLinePositionUpdate(this);
                }
                
            }
            get { return (Point)GetValue(PositionProperty); }
        }

        public List<EdgeVm> EdgeVmList { get; set; }
    }
}
