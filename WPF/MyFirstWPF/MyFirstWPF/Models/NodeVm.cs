using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MyFirstWPF.Models
{
    public class NodeVm
    {
        public static int NodeVmCount;

        public NodeVm()
        {
            Id = NodeVmCount;
            NodeVmCount++;
        }
        public int Id { get; set; }
        public TextBlock TextBlock { get; set; }
        public Node Node { get; set; }
        public Point Position { get; set; }
    }
}
