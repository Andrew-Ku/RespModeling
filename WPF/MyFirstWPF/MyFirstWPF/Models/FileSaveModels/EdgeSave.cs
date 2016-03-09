using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstWPF.Consts;

namespace MyFirstWPF.Models
{
    public class EdgeSave{

        public int Id { get; set; }

        public int FromNodeVmId { get; set; }

        public int ToNodeVmId { get; set; }

        public double X1 { get; set; }

        public double X2 { get; set; }

        public double Y1 { get; set; }

        public double Y2 { get; set; }

        public ArrowEnds ArrowEnds { get; set; }


    }
}
