using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly MathService _mathService;

        public List<NodeVm> NodeVmList;
        public List<EdgeVm> EdgeVmList;
        public List<Node> NodeList;
        public double NodeRadius;
        public NodeVm StartNodeVmEdge;
        public bool MoveFlag;
        public Point CurrPosition;
        public NodeVm MoveNodeVm;
        public NodeVm EditNodeVm;

        public MainWindow()
        {
            _nodeService = new NodeService();
            _mathService = new MathService();

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

            if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
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
                    Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, NodeColors.NormalBackground, NodeColors.NormalBorder))
                };

                textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
                textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
                textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
                textBlock.MouseMove += TextBlock_MouseMove;
                
                var node = new Node();
                if (node.IsStartNode)
                {
                    textBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, NodeColors.StartNodeBackground, NodeColors.NormalBorder));
                }
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
            e.Handled = true;

        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            NodeList.ClearEx();
            NodeVmList.ClearEx();
            LogTextBox.Clear();
            EdgeVmList.Clear();
            WorkPlaceCanvas.Children.Clear();
        }

        private void WorkPlaceCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            SetNormalNodeVmColor(ref StartNodeVmEdge);

            #region Создание связи

            if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                if (nodeVm == null || Equals(StartNodeVmEdge, nodeVm))
                {
                    SetNormalNodeVmColor(ref StartNodeVmEdge, true);
                    return;
                }
                else if (StartNodeVmEdge == null)
                {
                    StartNodeVmEdge = nodeVm;
                    StartNodeVmEdge.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, StartNodeVmEdge.Node.IsStartNode ? NodeColors.StartNodeBackground : NodeColors.NormalBackground, NodeColors.EdgeCreateBorder));
                }
                else
                {
                    // Есть ли такая связь
                    if (EdgeVmList.Any(ed => ed.FromNodeVm.Equals(StartNodeVmEdge) && ed.ToNodeVm.Equals(nodeVm)))
                    {
                        SetNormalNodeVmColor(ref StartNodeVmEdge);
                        return;
                    }
                    // Есть ли обратная связь
                    if (EdgeVmList.Any(ed => ed.FromNodeVm.Equals(nodeVm) && ed.ToNodeVm.Equals(StartNodeVmEdge)))
                    {

                        EdgeVmList.Single(ed => ed.FromNodeVm.Equals(nodeVm) && ed.ToNodeVm.Equals(StartNodeVmEdge))
                            .ArrowLine.ArrowEnds = ArrowEnds.Both;


                        StartNodeVmEdge.Node.NodeRelations.Add(new NodeRelation()
                        {
                            NodeId = nodeVm.Node.Id,
                            Weight = 1
                        });


                        SetNormalNodeVmColor(ref StartNodeVmEdge);
                        return;
                    }

                    StartNodeVmEdge.Node.NodeRelations.Add(new NodeRelation()
                    {
                        NodeId = nodeVm.Node.Id,
                        Weight = 1
                    });

                    var edgePos = NodeService.ReduceArrowLine(StartNodeVmEdge.Position, nodeVm.Position);

                    var edgeVm = new EdgeVm()
                    {
                        FromNodeVm = StartNodeVmEdge,
                        ToNodeVm = nodeVm,
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
                            X1 = edgePos.Item1.X,
                            Y1 = edgePos.Item1.Y,
                            X2 = edgePos.Item2.X,
                            Y2 = edgePos.Item2.Y,

                            Stroke = Brushes.Black,
                            StrokeThickness = 2
                        }
                    };

                    StartNodeVmEdge.EdgeVmList.Add(edgeVm);
                    nodeVm.EdgeVmList.Add(edgeVm);

                    EdgeVmList.Add(edgeVm);

                    WorkPlaceCanvas.Children.Add(edgeVm.ArrowLine);
                    //WorkPlaceCanvas.Children.Add(edgeVm.FromWeightLabel);
                    SetNormalNodeVmColor(ref StartNodeVmEdge, true);
                }

            }

            #endregion
            e.Handled = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetNormalNodeVmColor(ref StartNodeVmEdge, true);
            SetNormalNodeVmColor(ref EditNodeVm);

            if (Keyboard.IsKeyDown(Key.D))
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                DeleteNode(nodeVm);
            }
            else
            {
                if (CreateModeRadioButton.IsChecked == true)
                {
                    CurrPosition = e.GetPosition(WorkPlaceCanvas);
                    MoveFlag = true;
                    MoveNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                    MoveNodeVm.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, MoveNodeVm.Node.IsStartNode ? NodeColors.StartNodeBackground : NodeColors.NormalBackground, NodeColors.MoveNodeBorder));
                    Mouse.Capture(MoveNodeVm.TextBlock);
                    MoveFlagLabel.Content = MoveFlag.ToString();
                }
                if (EditModeRadioButton.IsChecked == true)
                {
                    EditNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                    EditNodeVm.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, EditNodeVm.Node.IsStartNode ? NodeColors.StartNodeBackground : NodeColors.NormalBackground, NodeColors.EditNodeBorder));
                    NodeNumberTextBox.Text = EditNodeVm.Node.Id.ToString();
                    StartNodeCheckBox.IsChecked = EditNodeVm.Node.IsStartNode;
                    RejectionNodeCheckBox.IsChecked = EditNodeVm.Node.IsRejectionNode;

                    if (EditNodeVm.Node.IsStartNode)
                    {
                        StartNodeCheckBox.IsEnabled = false;
                        RejectionNodeCheckBox.IsEnabled = false;
                    }
                    else
                    {
                        StartNodeCheckBox.IsEnabled = true;
                        RejectionNodeCheckBox.IsEnabled = true;
                    }
                }
            }

            e.Handled = true;
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MoveNodeVm == null) return;
            Mouse.Capture(null);
            MoveFlag = false;
            MoveFlagLabel.Content = MoveFlag.ToString();
            MoveNodeVm.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, MoveNodeVm.Node.IsStartNode ? NodeColors.StartNodeBackground : NodeColors.NormalBackground, Brushes.Tan));
            MoveNodeVm = null;
            e.Handled = true;
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            MoveXPosLabel.Content = e.GetPosition(WorkPlaceCanvas).X;
            MoveYPosLabel.Content = e.GetPosition(WorkPlaceCanvas).Y;

            if (MoveFlag)
            {
                var cursorPosition = e.GetPosition(WorkPlaceCanvas);
                MoveNodeVm.TextBlock.Margin = new Thickness(cursorPosition.X - NodeRadius, cursorPosition.Y - NodeRadius, 0, 0);
                MoveNodeVm.Position = new Point(cursorPosition.X, cursorPosition.Y);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Установка обыного цвета вершины
        /// </summary>
        private void SetNormalNodeVmColor(ref NodeVm nodeVm, bool nullFlag = false)
        {
            if (nodeVm == null) return;
            nodeVm.TextBlock.Background =
                new VisualBrush(_nodeService.GetEllipse(NodeRadius, nodeVm.Node.IsStartNode ? NodeColors.StartNodeBackground : NodeColors.NormalBackground, NodeColors.NormalBorder));

            if (nullFlag)
                nodeVm = null;
        }


        /// <summary>
        /// Удаление всех данных об узле
        /// </summary>
        /// <param name="nodeVm"></param>
        private void DeleteNode(NodeVm nodeVm)
        {
            // Удаляем связи с данным узлом во всех узлах
            foreach (var node in NodeList)
            {
                node.NodeRelations.RemoveAll(n => n.NodeId == nodeVm.Node.Id);
            }

            // Поиск ребер у которого на одном из концов есть удалемый узел
            var deleteEdges = EdgeVmList.Where(e => e.FromNodeVm.Equals(nodeVm) || e.ToNodeVm.Equals(nodeVm)).ToList();
            if (deleteEdges.Any())
            {
                // Удаляем найденные ребра из рабочей области
                foreach (var edge in deleteEdges.ToList())
                {
                    WorkPlaceCanvas.Children.Remove(edge.ArrowLine);
                    WorkPlaceCanvas.Children.Remove(edge.FromWeightLabel);
                    WorkPlaceCanvas.Children.Remove(edge.ToWeightLabel);
                }
            }

            // Удаляем ребра из списка ребер узлов
            foreach (var vm in NodeVmList.Where(c => c.EdgeVmList.Any(deleteEdges.Contains)))
            {
                vm.EdgeVmList.RemoveAll(deleteEdges.Contains);
            }

            // Удаляем узел из рабочей области
            WorkPlaceCanvas.Children.Remove(nodeVm.TextBlock);

            // Удаляем элемнты из списков
            EdgeVmList.RemoveAll(deleteEdges.Contains);
            NodeList.Remove(nodeVm.Node);
            NodeVmList.Remove(nodeVm);

            SetNormalNodeVmColor(ref StartNodeVmEdge, true);
        }

        private void WorkPlaceCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            MoveXPosLabel.Content = e.GetPosition(WorkPlaceCanvas).X;
            MoveYPosLabel.Content = e.GetPosition(WorkPlaceCanvas).Y;
        }

        private void NodeNumberValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (EditNodeGrid == null) return;
            EditNodeGrid.Visibility = e.Source.Equals(EditModeRadioButton) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SaveEditNodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditNodeVm == null) return;
           
            if (NodeNumberTextBox.Text.Length == 2 && NodeNumberTextBox.Text.StartsWith("0"))
            {
                 NodeNumberTextBox.Text = NodeNumberTextBox.Text.Substring(1);
            }
             var number = int.Parse(NodeNumberTextBox.Text);

            if (NodeList.Any(n => n.Id == number) && number != EditNodeVm.Id)
            {
                MessageBox.Show("Узел с указанным номером уже существует", "Ошибка редактирования", MessageBoxButton.OK);
                return;
            }
            if (RejectionNodeCheckBox.IsChecked == true && StartNodeCheckBox.IsChecked == true)
            {
                MessageBox.Show("Узел не может быть стартовым и отказным одновременно", "Ошибка редактирования", MessageBoxButton.OK);
                return;
            }

            if (StartNodeCheckBox.IsChecked == true)
            {
                var oldStartNode = NodeList.Single(n => n.IsStartNode);
                oldStartNode.IsStartNode = false;
                var oldStartNodeVm = NodeVmList.Single(nv => nv.Node.Equals(oldStartNode));
                oldStartNodeVm.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, NodeColors.NormalBackground, NodeColors.NormalBorder));
           
                var newStartNodeVm = NodeVmList.Single(nv => nv.Equals(EditNodeVm));
                newStartNodeVm.TextBlock.Background = new VisualBrush(_nodeService.GetEllipse(NodeRadius, NodeColors.StartNodeBackground, NodeColors.NormalBorder));
            }

            ChangeNodeId(EditNodeVm.Id, number);
            EditNodeVm.Node.IsStartNode = StartNodeCheckBox.IsChecked.GetValueOrDefault();
            EditNodeVm.Node.IsRejectionNode = RejectionNodeCheckBox.IsChecked.GetValueOrDefault();
        }


        private void ChangeNodeId(int oldId, int newId)
        {
            var node = NodeList.Single(n => n.Id == oldId);
            var nodeVm = NodeVmList.Single(nv => nv.Node.Equals(node));
            nodeVm.Id = newId;
            nodeVm.TextBlock.Text = newId.ToString();

            EditNodeVm.Node.Id = newId;

            foreach (var relation in NodeList.Select(nodeL => nodeL.NodeRelations.Where(n => n.NodeId == oldId).ToList()).SelectMany(relations => relations))
            {
                relation.NodeId = newId;
            }
        }
    }


}
