using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyFirstWPF.Consts;
using MyFirstWPF.Extensions;
using MyFirstWPF.Models;
using MyFirstWPF.Services;

namespace MyFirstWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly NodeService _nodeService;

        public List<NodeVm> NodeVmList;
        public List<EdgeVm> EdgeVmList;
        public List<Node> NodeList;
        public double NodeRadius;
        public NodeVm StartNodeVmEdge;
        public bool MoveFlag;
        public Point CurrPosition;
       
        public MainWindow()
        {
            _nodeService = new NodeService();

            NodeVmList = new List<NodeVm>();
            NodeList = new List<Node>();
            NodeRadius = StartParameters.NodeRadius;
            EdgeVmList = new List<EdgeVm>();
            MoveFlag = false;


            InitializeComponent();
        }

        private void WorkPlaceCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            #region Добавление нового узла

            if (StateCheckBox.IsChecked.GetValueOrDefault())
            {
                var cursorPosition = e.GetPosition(WorkPlaceCanvas);

                if (NodeVmList.Any(n => Math.Abs(n.Position.X - cursorPosition.X) < NodeRadius * 2 && Math.Abs(n.Position.Y - cursorPosition.Y) < NodeRadius * 2)) return;

                var paddingLeft = Node.NodeCount > 9 ? 12 : 17;
                var textBlock = new TextBlock()
                {
                    Text = Node.NodeCount.ToString(),
                    Height = NodeRadius * 2.0,
                    Width = NodeRadius * 2.0,
                    Padding = new Thickness(paddingLeft, NodeRadius / 2.0, 0, 0),
                    Margin = new Thickness(cursorPosition.X - NodeRadius, cursorPosition.Y - NodeRadius, 0.0, 0.0),
                    Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, Brushes.Moccasin, Brushes.Tan))
                };

                textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
                textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
                textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
                textBlock.MouseMove += TextBlock_MouseMove;



                var node = new Node();
                var nodeVm = new NodeVm()
                {
                    TextBlock = textBlock,
                    Node = node,
                    Position = cursorPosition
                };

                NodeList.Add(node);
                NodeVmList.Add(nodeVm);

                WorkPlaceCanvas.Children.Add(nodeVm.TextBlock);
            }
            #endregion

            #region
            #endregion

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in NodeVmList.Select(item => item.Position))
            {
                WorkPlaceCanvas.Children.Add(new ArrowLine()
                {
                    X1 = p.X,
                    Y1 = p.Y,
                    X2 = 100,
                    Y2 = 100,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                });
            }
        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            NodeList.ClearEx();
            NodeVmList.ClearEx();

            WorkPlaceCanvas.Children.Clear();

        }

        private void WorkPlaceCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
         
        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            AddEdgeClear();

            #region Создание связи

            if (StateCheckBox.IsChecked.GetValueOrDefault())
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                if (nodeVm == null || StartNodeVmEdge == nodeVm)
                {
                    AddEdgeClear();
                    StartNodeVmEdge = null;
                    return;
                }
                else if (StartNodeVmEdge == null)
                {
                    StartNodeVmEdge = nodeVm;
                    StartNodeVmEdge.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, Brushes.Moccasin, Brushes.Red));
                }
                else
                {
                    StartNodeVmEdge.Node.NodeRelations.Add(new NodeRelation()
                    {
                        NodeId = nodeVm.Node.Id,
                        Weight = 1
                    });

                    var edgeVm = new EdgeVm()
                    {
                        FromNode = StartNodeVmEdge,
                        ToNode = nodeVm,
                        FromWeightLabel = new Label()
                        {
                            Content = StartNodeVmEdge.Node.Id,
                            Margin =
                                new Thickness(StartNodeVmEdge.Position.X + 10, StartNodeVmEdge.Position.Y + 10, 0, 0)
                        },
                        ToWeightLabel = new Label()
                        {
                            Content = "",
                            Margin =
                                new Thickness(StartNodeVmEdge.Position.X + 10, StartNodeVmEdge.Position.Y + 10, 0, 0)
                        },
                        ArrowLine = new ArrowLine()
                        {
                            X1 = StartNodeVmEdge.Position.X,
                            Y1 = StartNodeVmEdge.Position.Y,
                            X2 = nodeVm.Position.X,
                            Y2 = nodeVm.Position.Y,
                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        }
                    };

                    EdgeVmList.Add(edgeVm);

                    WorkPlaceCanvas.Children.Add(edgeVm.ArrowLine);
                    WorkPlaceCanvas.Children.Add(edgeVm.FromWeightLabel);
                    AddEdgeClear();
                    StartNodeVmEdge = null;
                }

            }

            #endregion
            e.Handled = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CurrPosition = e.GetPosition(WorkPlaceCanvas);
            MoveFlag = true;
          
            e.Handled = true;
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CurrPosition = e.GetPosition(WorkPlaceCanvas);
            MoveFlag = false;

            e.Handled = true;
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveFlag)
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));

                nodeVm.TextBlock.SetValue(Canvas.LeftProperty, e.GetPosition(WorkPlaceCanvas).X-CurrPosition.X);
                nodeVm.TextBlock.SetValue(Canvas.TopProperty, e.GetPosition(WorkPlaceCanvas).Y - CurrPosition.Y);
            }

            e.Handled = true;
        }

        private void AddEdgeClear()
        {
            if (StartNodeVmEdge == null) return;
            StartNodeVmEdge.TextBlock.Background =
                    new VisualBrush(_nodeService.GetEllipse(NodeRadius, Brushes.Moccasin, Brushes.Tan));

        }
    }


}
