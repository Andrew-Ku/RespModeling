using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstWPF.Models;

namespace MyFirstWPF.ViewModel
{
    public class MainWindowVm
    {
        public List<NodeVm> NodeVmList;
        public List<EdgeVm> EdgeVmList;
        public List<Node> NodeList;

        public MainWindowVm()
        {
            NodeVmList = new List<NodeVm>();
            EdgeVmList = new List<EdgeVm>();
            NodeList = new List<Node>();
            //NodeList = new List<Node>()
            //{
            //    new Node()
            //    {
            //        Id = 12
            //    }
            //};

            //Node = new Node()
            //{
            //    Id = 1
            //};
        }

    }


}
