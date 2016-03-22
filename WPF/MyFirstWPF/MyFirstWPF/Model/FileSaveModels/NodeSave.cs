using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyFirstWPF.Models
{
    [Serializable]
    public class NodeSave
    {
        /// <summary>
        /// Id узла
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Позиция узла на рабочем поле
        /// </summary>
        public Point Position { get; set; }

    

        /// <summary>
        /// Стартовый узел
        /// </summary>
        public bool IsStartNode { get; set; }

        /// <summary>
        /// Отказной узел
        /// </summary>
        public bool IsRejectionNode { get; set; }

        /// <summary>
        /// Связи узлов
        /// </summary>
        public List<NodeRelation> NodeRelations { get; set; }

        /// <summary>
        /// Имя текстового блока для канвас
        /// </summary>
        public string TextBlockName { get; set; }

        /// <summary>
        /// Содержимое узла на канвас
        /// </summary>
        public string TextBlockText { get; set; }
    }
}
