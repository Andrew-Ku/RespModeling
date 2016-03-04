using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Xaml;
using AutoMapper;
using Microsoft.Win32;
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
        #region Initialization

        private readonly NodeService _nodeService;
        private readonly FileService _fileService;

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
            _fileService = new FileService();

            NodeVmList = new List<NodeVm>();
            NodeList = new List<Node>();
            NodeRadius = StartParameters.NodeRadius;
            EdgeVmList = new List<EdgeVm>();
            MoveFlag = false;

            InitializeComponent();
        }

        #endregion Initialization

        #region WorkPlaceCanvas Handle events
        private void WorkPlaceCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
            {
                var cursorPosition = e.GetPosition(WorkPlaceCanvas);

                if (cursorPosition.X <= NodeRadius || cursorPosition.X >= WorkPlaceCanvas.ActualWidth - NodeRadius) return;
                if (cursorPosition.Y <= NodeRadius || cursorPosition.Y >= WorkPlaceCanvas.ActualHeight - NodeRadius) return;


                if (NodeVmList.Any(n => Math.Abs(n.Position.X - cursorPosition.X) < NodeRadius * 2 && Math.Abs(n.Position.Y - cursorPosition.Y) < NodeRadius * 2)) return;

                var paddingLeft = Node.NodeCount > 9 ? 12 : 17;

                var textBlock = new TextBlock()
                {
                    Name = "TextBlock" + Node.NodeCount,
                    Text = Node.NodeCount.ToString(),
                    Height = NodeRadius * 2.0,
                    Width = NodeRadius * 2.0,
                    Padding = new Thickness(paddingLeft, NodeRadius / 2.0, 0, 0),
                    Margin = new Thickness(cursorPosition.X - NodeRadius, cursorPosition.Y - NodeRadius, 0.0, 0.0)
                };

                textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
                textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
                textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
                textBlock.MouseMove += TextBlock_MouseMove;

                WorkPlaceCanvas.RegisterName(textBlock.Name, textBlock);


                var node = new Node();
                var nodeVm = new NodeVm()
                {
                    TextBlock = textBlock,
                    Node = node,
                    Position = cursorPosition
                };

                NodeList.Add(node);
                NodeVmList.Add(nodeVm);
             

                NodeService.SetNodeVmColor(ref nodeVm, border: NodeColors.NormalBorder);

                WorkPlaceCanvas.Children.Add(nodeVm.TextBlock);

                _fileService.SerializeObject(nodeVm);
            }
            e.Handled = true;

        }

        private void WorkPlaceCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void WorkPlaceCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            MoveXPosLabel.Content = e.GetPosition(WorkPlaceCanvas).X.ToString("####");
            MoveYPosLabel.Content = e.GetPosition(WorkPlaceCanvas).Y.ToString("####");
        }

        #endregion WorkPlaceCanvas Handle events

        #region TextBlock Handle events

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NodeService.SetNodeVmColor(ref StartNodeVmEdge);

            #region Создание связи

            if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                if (nodeVm == null || Equals(StartNodeVmEdge, nodeVm))
                {
                    NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
                    return;
                }
                else if (StartNodeVmEdge == null)
                {
                    StartNodeVmEdge = nodeVm;
                    NodeService.SetNodeVmColor(ref StartNodeVmEdge, border: NodeColors.EdgeCreateBorder);

                }
                else
                {
                    // Есть ли такая связь
                    if (EdgeVmList.Any(ed => ed.FromNodeVm.Equals(StartNodeVmEdge) && ed.ToNodeVm.Equals(nodeVm)))
                    {
                        NodeService.SetNodeVmColor(ref StartNodeVmEdge);
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


                        NodeService.SetNodeVmColor(ref StartNodeVmEdge);
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
                    NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
                }

            }

            #endregion
            e.Handled = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
            NodeService.SetNodeVmColor(ref EditNodeVm);

            if (Keyboard.IsKeyDown(Key.D))
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));

                _fileService.SerializeObject(nodeVm);
                DeleteNode(nodeVm);
            }
            else
            {
                if (CreateModeRadioButton.IsChecked == true)
                {
                    CurrPosition = e.GetPosition(WorkPlaceCanvas);
                    MoveFlag = true;
                    MoveNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                    NodeService.SetNodeVmColor(ref MoveNodeVm, border: NodeColors.MoveNodeBorder);
                    Mouse.Capture(MoveNodeVm.TextBlock);
                    MoveFlagLabel.Content = MoveFlag.ToString();
                }
                if (EditModeRadioButton.IsChecked == true)
                {
                    EditNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                    NodeService.SetNodeVmColor(ref EditNodeVm, border: NodeColors.EditNodeBorder);
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
            NodeService.SetNodeVmColor(ref MoveNodeVm, border: NodeColors.NormalBorder);
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

                if (cursorPosition.X <= NodeRadius || cursorPosition.X >= WorkPlaceCanvas.ActualWidth - NodeRadius) return;
                if (cursorPosition.Y <= NodeRadius || cursorPosition.Y >= WorkPlaceCanvas.ActualHeight - NodeRadius) return;

                MoveNodeVm.TextBlock.Margin = new Thickness(cursorPosition.X - NodeRadius, cursorPosition.Y - NodeRadius, 0, 0);
                MoveNodeVm.Position = new Point(cursorPosition.X, cursorPosition.Y);
            }

            e.Handled = true;
        }

        #endregion TextBlock Handle events

        #region ButtonClick Handle events

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            var s = WorkPlaceCanvas.FindName("TextBlock0");
            NodeList.ClearEx();
            NodeVmList.ClearEx();
            LogTextBox.Clear();
            EdgeVmList.Clear();
            WorkPlaceCanvas.Children.Clear();
        }

        private void SaveEditNodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditNodeVm == null) return;

            if (NodeNumberTextBox.Text.Length == 2 && NodeNumberTextBox.Text.StartsWith("0"))
            {
                NodeNumberTextBox.Text = NodeNumberTextBox.Text.Substring(1);
            }
            var number = int.Parse(NodeNumberTextBox.Text);

            if (NodeList.Any(n => n.Id == number) && number != EditNodeVm.Node.Id)
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
                oldStartNodeVm.TextBlock.Background = new VisualBrush(NodeService.GetEllipse(NodeRadius, NodeColors.NormalBackground, NodeColors.NormalBorder));
                //  NodeService.SetNodeVmColor(ref oldStartNodeVm, border: NodeColors.NormalBorder);
                var newStartNodeVm = NodeVmList.Single(nv => nv.Equals(EditNodeVm));
                newStartNodeVm.TextBlock.Background = new VisualBrush(NodeService.GetEllipse(NodeRadius, NodeColors.StartNodeBackground, NodeColors.NormalBorder));
                //     NodeService.SetNodeVmColor(ref newStartNodeVm, border: NodeColors.NormalBorder);
            }

            ChangeNodeId(EditNodeVm.Node.Id, number);
            EditNodeVm.Node.IsStartNode = StartNodeCheckBox.IsChecked.GetValueOrDefault();
            EditNodeVm.Node.IsRejectionNode = RejectionNodeCheckBox.IsChecked.GetValueOrDefault();

            NodeService.SetNodeVmColor(ref EditNodeVm, border: NodeColors.NormalBorder);
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
           var  fileDialog = new OpenFileDialog
           {
               DefaultExt = ".xaml",
               Filter = " (.xaml)|*.xaml"
           };

           var result = fileDialog.ShowDialog();
         
            if (result == true)
            {
                var filename = fileDialog.FileName;

                this.Content = _fileService.OpenFile(filename);
            }
        }

        

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
          
            
            // Properties.Settings.Default.WindowSetting = new Window();
          


            _fileService.SaveFile(this);
        }

        #endregion TextBlock Handle events

        #region Others

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

        #endregion Others

        #region Helpful methods

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

            if (NodeList.Max(n => n.Id) == nodeVm.Node.Id)
            {
                Node.NodeCount--;
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

            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
        }

        private void ChangeNodeId(int oldId, int newId)
        {
            var node = NodeList.Single(n => n.Id == oldId);
            var nodeVm = NodeVmList.Single(nv => nv.Node.Equals(node));
            nodeVm.TextBlock.Name = "TextBlock" + newId;
            nodeVm.TextBlock.Text = newId.ToString();
            var paddingLeft = newId > 9 ? 12 : 17;
            nodeVm.TextBlock.Padding = new Thickness(paddingLeft, NodeRadius / 2.0, 0, 0);

            if (NodeList.Max(n => n.Id) < newId)
            {
                Node.NodeCount = newId + 1;
            }
            EditNodeVm.Node.Id = newId;

            foreach (var relation in NodeList.Select(nodeL => nodeL.NodeRelations.Where(n => n.NodeId == oldId).ToList()).SelectMany(relations => relations))
            {
                relation.NodeId = newId;
            }
        }

        #endregion Helpful methods


    }
}
