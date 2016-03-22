using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWPF.Models.Generation
{
    public class GpssInputModel
    {
        public List<Node> Nodes;

        public int ModelingTime { get; set; }

        public int ObservationTime { get; set; }
    }
}
