using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstWPF.Models;

namespace MyFirstWPF.Extensions
{
    public static class ListExtensions
    {
        public static void ClearEx(this List<NodeVm> list)
        {
            NodeVm.NodeVmCount = 0;
            list.Clear();
        }
        public static void ClearEx(this List<Node> list)
        {
            Node.NodeCount = 0;
            Node.IsSetupStartNode = false;
            list.Clear();
        }
    }
}
