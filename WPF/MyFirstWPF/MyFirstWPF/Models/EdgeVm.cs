using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyFirstWPF.Consts;
using MyFirstWPF.Services;

namespace MyFirstWPF.Models
{
    public class EdgeVm
    {
        public int Id { get; set; }

        public ArrowLine ArrowLine { get; set; }

        public Label FromWeightLabel { get; set; }
        public Label ToWeightLabel { get; set; }

        public NodeVm FromNodeVm { get; set; }
        public NodeVm ToNodeVm { get; set; }


        public void ArrowLinePositionUpdate(NodeVm nodeVm)
        {

            if (nodeVm.Equals(FromNodeVm))
            {
                //ArrowLine.X1 = nodeVm.Position.X;
                //ArrowLine.Y1 = nodeVm.Position.Y;

                var pos = NodeService.ReduceArrowLine(FromNodeVm.Position, ToNodeVm.Position);

                ArrowLine.X1 = pos.Item1.X;
                ArrowLine.Y1 = pos.Item1.Y;

                ArrowLine.X2 = pos.Item2.X;
                ArrowLine.Y2 = pos.Item2.Y;


            }
            if (nodeVm.Equals(ToNodeVm))
            {
                var pos = NodeService.ReduceArrowLine(ToNodeVm.Position, FromNodeVm.Position);

                ArrowLine.X1 = pos.Item2.X;
                ArrowLine.Y1 = pos.Item2.Y;

                ArrowLine.X2 = pos.Item1.X;
                ArrowLine.Y2 = pos.Item1.Y;
            }
        }
    }
}
