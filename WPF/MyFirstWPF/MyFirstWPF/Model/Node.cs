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
    public class Node : INotifyPropertyChanged
    {

        private int _Id;
   

        public static bool IsSetupStartNode;
        public static int NodeCount;

        #region Implement INotyfyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
 

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

        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public List<NodeRelation> NodeRelations { get; set; }
        public bool IsStartNode { get; set; }
        public bool IsRejectionNode { get; set; }
    }
}
