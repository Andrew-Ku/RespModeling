using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    }
}
