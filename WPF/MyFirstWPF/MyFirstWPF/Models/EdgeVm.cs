using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MyFirstWPF.Consts;
using MyFirstWPF.Services;
using Newtonsoft.Json;

namespace MyFirstWPF.Models
{
    [Serializable]
    public class EdgeVm
    {
        public int FromNodeVmId { get; set; }
       
        public int ToNodeVmId { get; set; }

        [JsonIgnore] 
        public ArrowLine ArrowLine { get; set; }

        [JsonIgnore] 
        public Label FromWeightLabel { get; set; }

        [JsonIgnore] 
        public Label ToWeightLabel { get; set; }

        [JsonIgnore] 
        public NodeVm FromNodeVm { get; set; }

        [JsonIgnore] 
        public NodeVm ToNodeVm { get; set; }


        public void ArrowLinePositionUpdate(NodeVm nodeVm)
        {
            if (nodeVm.Equals(FromNodeVm))
            {
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
