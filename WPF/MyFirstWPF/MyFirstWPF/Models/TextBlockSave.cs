using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyFirstWPF.Models
{
    [Serializable]
    public class TextBlockSave
    {
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
