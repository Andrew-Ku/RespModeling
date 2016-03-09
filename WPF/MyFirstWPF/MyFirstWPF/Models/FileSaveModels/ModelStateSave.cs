using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWPF.Models
{
    public class ModelStateSave
    {
        public List<NodeSave> NodeSaveList { get; set; }

        public List<EdgeSave> EdgeSaveList { get; set; }
        public int NodeCount { get; set; }
        public bool IsSetupStartNode { get; set; }
    

        public ModelStateSave()
        {
            NodeSaveList = new List<NodeSave>();
            EdgeSaveList = new List<EdgeSave>();
        }
    }
}
