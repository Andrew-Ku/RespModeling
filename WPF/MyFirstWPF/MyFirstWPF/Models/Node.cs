using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MyFirstWPF.Models
{
    [Description("Узел состояния")]
    public class Node
    {
        public static bool IsSetupStartNode;
        public static int NodeCount;

        public Node()
        {
            Id = NodeCount;
            IsRejectionNode = false;
            if (!IsSetupStartNode)
            {
                IsStartNode = true;
                IsSetupStartNode = true;
            }

            NodeRelations = new List<NodeRelation>();
            NodeCount++;
        }

        public Node(int id, bool isStartNode, bool isRejectionNode, List<NodeRelation> nodeRelations )
        {
            Id = id;
            IsStartNode = isStartNode;
            IsRejectionNode = isRejectionNode;
          
            NodeCount++;
            NodeRelations = nodeRelations;
        }

        public int Id { get; set; }
        public List<NodeRelation> NodeRelations { get; set; }
        public bool IsStartNode { get; set; }
        public bool IsRejectionNode { get; set; }
    }
}
