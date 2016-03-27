using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using MyFirstWPF.Consts;
using MyFirstWPF.Models;

namespace MyFirstWPF.Services
{
    public class NodeService
    {
        public Ellipse GetEllipse(double radius, Brush ellipseColor, Brush borderColor)
        {
            return new Ellipse()
            {
                Height = radius,
                Width = radius,
                Fill = ellipseColor,
                Stroke = borderColor
            };
        }

        /// <summary>
        /// Установка цета узла
        /// </summary>
        public void SetNodeVmColor(ref NodeVm nodeVm, bool nullFlag = false, Brush border = null)
        {
            if (nodeVm == null) return;

            nodeVm.TextBlock.Background = new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.NormalBackground, border ?? NodeColors.NormalBorder));

            if (nodeVm.Node.IsStartNode)
            {
                nodeVm.TextBlock.Background =
               new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.StartNodeBackground, border ?? NodeColors.NormalBorder));
            }

            if (nodeVm.Node.IsRejectionNode)
            {
                nodeVm.TextBlock.Background =
               new VisualBrush(GetEllipse(StartParameters.NodeRadius, NodeColors.RejectionNodeBackground, border ?? NodeColors.NormalBorder));
            }

            if (nullFlag)
                nodeVm = null;
        }

        public void SetEdgeVmColor(ref EdgeVm edgeVm, bool nullFlag = false)
        {
            if (edgeVm == null) return;

            edgeVm.ArrowLine.Stroke = EdgeColors.NormalEdge;

            if (nullFlag)
                edgeVm = null;
        }
    }
}
