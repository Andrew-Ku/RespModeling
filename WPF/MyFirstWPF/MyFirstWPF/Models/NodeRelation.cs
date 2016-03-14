using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWPF.Models
{
    public class NodeRelation
    {
        [DisplayName("Узел")]
        public int NodeId { get; set; }

        [DisplayName("Вес")]
        public double Weight { get; set; }
    }
}
