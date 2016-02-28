using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyFirstWPF.Models
{
    public class EdgeVm
    {
        public int Id { get; set; }

        public ArrowLine ArrowLine { get; set; }

        public Label FromWeightLabel { get; set; }
        public Label ToWeightLabel { get; set; }

        public NodeVm FromNode { get; set; }
        public NodeVm ToNode { get; set; }
    }
}
