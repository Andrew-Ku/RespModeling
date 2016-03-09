using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoMapper;
using Microsoft.Win32;
using MyFirstWPF.Consts;
using MyFirstWPF.Extensions;
using MyFirstWPF.Infrastructure;
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
        public EdgeVm EditEdgeVm;

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
            ClearViewModel();
            ClearAllInputElement();

            //if (Keyboard.IsKeyDown(Key.D))
            //{
            //    e.Handled = true;
            //    return;
            //}

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


                SetTextBlockEventHandles(textBlock);

                //     WorkPlaceCanvas.RegisterName(textBlock.Name, textBlock);


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

        private void WorkPlaceCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                EditModeRadioButton.IsChecked = true;
            else
                CreateModeRadioButton.IsChecked = true;
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
                        FromNodeVmId = StartNodeVmEdge.Node.Id,
                        FromNodeVm = StartNodeVmEdge,
                        ToNodeVmId = nodeVm.Node.Id,
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

                            Stroke = EdgeColors.NormalEdge,
                            StrokeThickness = EdgeThickness.NormalThickness
                        }
                    };

                    SetArrowLineEventHandles(edgeVm.ArrowLine);
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
            NodeService.SetEdgeVmColor(ref EditEdgeVm, true);

            if (Keyboard.IsKeyDown(Key.D))
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                DeleteNode(nodeVm);
                e.Handled = true;
                return;
            }
            if (Keyboard.IsKeyDown(Key.R))
            {
                var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                nodeVm.Node.IsRejectionNode = true;
                e.Handled = true;
                return;
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

        #region ArrowLine Hande events

        private void ArrowLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NodeService.SetEdgeVmColor(ref EditEdgeVm, true);

            if (Keyboard.IsKeyDown(Key.D))
            {
                var edgeVm = EdgeVmList.Single(n => Equals(n.ArrowLine, sender));
                DeleteEdge(edgeVm);
                e.Handled = true;
                return;
            }

            if (EditModeRadioButton.IsChecked.GetValueOrDefault())
            {
                var arrowLine = sender as ArrowLine;
                arrowLine.Stroke = EdgeColors.SelectEdge;
                EditEdgeVm = EdgeVmList.Single(ed => ed.ArrowLine.Equals(arrowLine));
                var toNodeVm = EditEdgeVm.ToNodeVm;
                var fromNodeVm = EditEdgeVm.FromNodeVm;

                var weightFrom = fromNodeVm.Node.NodeRelations.SingleOrDefault(r => r.NodeId == toNodeVm.Node.Id);
                var weightTo = toNodeVm.Node.NodeRelations.SingleOrDefault(r => r.NodeId == fromNodeVm.Node.Id);

                if (weightFrom != null)
                {
                    EdgeLambdaTextBox1.Text = weightFrom.Weight.ToString();
                    EdgeEditLabel1.Content = string.Format("Из {0} в {1}", fromNodeVm.Node.Id, toNodeVm.Node.Id);
                }
                else
                {
                    EdgeLambdaTextBox1.Text = string.Empty;
                    EdgeEditLabel1.Content = string.Format("Из {1} в {0}", fromNodeVm.Node.Id, toNodeVm.Node.Id);
                }

                if (weightTo != null)
                {
                    EdgeLambdaTextBox2.Text = weightTo.Weight.ToString();
                    EdgeEditLabel2.Content = string.Format("Из {0} в {1}", toNodeVm.Node.Id, fromNodeVm.Node.Id);
                }
                else
                {
                    EdgeLambdaTextBox2.Text = string.Empty;
                    EdgeEditLabel2.Content = string.Format("Из {1} в {0}", fromNodeVm.Node.Id, toNodeVm.Node.Id);
                }

                e.Handled = true;

            }
        }

        private void ArrowLine_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;

        }

        private void ArrowLine_MouseEnter(object sender, MouseEventArgs e)
        {
            if (EditModeRadioButton.IsChecked.GetValueOrDefault())
            {
                Cursor = Cursors.Hand;
            }
        }

        #endregion

        #region ButtonClick Handle events

        private void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (NodeList.All(n => !n.IsRejectionNode))
            {
                MessageBox.Show("Обязательно должно быть задано хотябы одно отказное состояние");
                return;
            }
            if (!NodeList.Any())
            {
                MessageBox.Show("Отсутсвует модель для генерации");
                return;
            }
            var genWindow = new StartGpssWindow(NodeList);
            genWindow.ShowDialog();

        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClearModel();
        }

        private void SaveEditEdgeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditEdgeVm == null) return;

            var toNodeVm = EditEdgeVm.ToNodeVm;
            var fromNodeVm = EditEdgeVm.FromNodeVm;

            var relationFrom = fromNodeVm.Node.NodeRelations.SingleOrDefault(r => r.NodeId == toNodeVm.Node.Id);
            var relationTo = toNodeVm.Node.NodeRelations.SingleOrDefault(r => r.NodeId == fromNodeVm.Node.Id);


            if (relationFrom != null && relationTo != null)
            {
                if (EdgeLambdaTextBox1.Text.Length == 0 || EdgeLambdaTextBox2.Text.Length == 0)
                {
                    MessageBox.Show("Все веса должны быть заданы");
                    return;
                }

                var weight1 = double.Parse(EdgeLambdaTextBox1.Text, CultureInfo.InvariantCulture);
                var weight2 = double.Parse(EdgeLambdaTextBox2.Text, CultureInfo.InvariantCulture);

                if (weight1 == 0.0 || weight2 == 0.0)
                {
                    MessageBox.Show("Ребра графа должны иметь веса отличные от нуля");
                    return;
                }

                relationFrom.Weight = weight1;
                relationTo.Weight = weight2;
            }

            if (relationTo == null)
            {
                if (EdgeLambdaTextBox1.Text.Length == 0)
                {
                    MessageBox.Show("Все веса должны быть заданы");
                    return;
                }

                var weight1 = double.Parse(EdgeLambdaTextBox1.Text, CultureInfo.InvariantCulture);

                if (weight1 == 0.0)
                {
                    MessageBox.Show("Ребра графа должны иметь веса отличные от нуля");
                    return;
                }

                relationFrom.Weight = weight1;
            }

            if (relationFrom == null)
            {
                if (EdgeLambdaTextBox2.Text.Length == 0)
                {
                    MessageBox.Show("Все веса должны быть заданы");
                    return;
                }

                var weight2 = double.Parse(EdgeLambdaTextBox2.Text, CultureInfo.InvariantCulture);

                if (weight2 == 0.0)
                {
                    MessageBox.Show("Ребра графа должны иметь веса отличные от нуля");
                    return;
                }

                relationTo.Weight = weight2;
            }

            EdgeLambdaTextBox1.Text = "";
            EdgeLambdaTextBox2.Text = "";
            NodeService.SetEdgeVmColor(ref EditEdgeVm);


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
                var newStartNodeVm = NodeVmList.Single(nv => nv.Equals(EditNodeVm));
                newStartNodeVm.TextBlock.Background = new VisualBrush(NodeService.GetEllipse(NodeRadius, NodeColors.StartNodeBackground, NodeColors.NormalBorder));

            }

            ChangeNodeId(EditNodeVm.Node.Id, number);
            EditNodeVm.Node.IsStartNode = StartNodeCheckBox.IsChecked.GetValueOrDefault();
            EditNodeVm.Node.IsRejectionNode = RejectionNodeCheckBox.IsChecked.GetValueOrDefault();

            NodeNumberTextBox.Clear();
            NodeService.SetNodeVmColor(ref EditNodeVm, border: NodeColors.NormalBorder);
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = " (.json)|*.json",
                InitialDirectory = Environment.CurrentDirectory + "\\Json"
            };

            if (fileDialog.ShowDialog().GetValueOrDefault())
            {
                var modelState = _fileService.OpenFile(fileDialog.FileName);

                if (modelState == null)
                {
                    MessageBox.Show("Ошибка при загрузке модели");
                    return;
                }

                ClearModel();
                ClearViewModel();

                Node.IsSetupStartNode = modelState.IsSetupStartNode;

                foreach (var item in modelState.NodeSaveList)
                {
                    var node = Mapper.Map(item, new Node());
                    var nodeVm = Mapper.Map(item, new NodeVm());
                    nodeVm.Node = node;
                    var paddingLeft = Node.NodeCount > 9 ? 12 : 17;

                    nodeVm.TextBlock = new TextBlock()
                    {
                        Name = item.TextBlockName,
                        Text = item.TextBlockText,
                        Height = NodeRadius * 2.0,
                        Width = NodeRadius * 2.0,
                        Padding = new Thickness(paddingLeft, NodeRadius / 2.0, 0, 0),
                        Margin = new Thickness(item.Position.X - NodeRadius, item.Position.Y - NodeRadius, 0.0, 0.0),
                    };

                    NodeService.SetNodeVmColor(ref nodeVm, border: NodeColors.NormalBorder);
                    SetTextBlockEventHandles(nodeVm.TextBlock);
                    //   WorkPlaceCanvas.RegisterName(nodeVm.TextBlock.Name, nodeVm.TextBlock);
                    WorkPlaceCanvas.Children.Add(nodeVm.TextBlock);
                    NodeList.Add(node);
                    NodeVmList.Add(nodeVm);
                }

                foreach (var item in modelState.EdgeSaveList)
                {
                    var edgeVm = Mapper.Map(item, new EdgeVm());
                    var fromNodeVm = NodeVmList.Single(n => n.Id == edgeVm.FromNodeVmId);
                    var toNodeVm = NodeVmList.Single(n => n.Id == edgeVm.ToNodeVmId);

                    fromNodeVm.EdgeVmList.Add(edgeVm);
                    toNodeVm.EdgeVmList.Add(edgeVm);

                    edgeVm.FromNodeVm = fromNodeVm;
                    edgeVm.ToNodeVm = toNodeVm;
                    edgeVm.ArrowLine = new ArrowLine()
                    {
                        X1 = item.X1,
                        X2 = item.X2,
                        Y1 = item.Y1,
                        Y2 = item.Y2,
                        ArrowEnds = item.ArrowEnds,
                        Stroke = EdgeColors.NormalEdge,
                        StrokeThickness = EdgeThickness.NormalThickness
                    };

                    SetArrowLineEventHandles(edgeVm.ArrowLine);
                    EdgeVmList.Add(edgeVm);
                    WorkPlaceCanvas.Children.Add(edgeVm.ArrowLine);
                }
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!NodeList.Any()) return;

            var modelState = new ModelStateSave()
            {
                IsSetupStartNode = Node.IsSetupStartNode,
                NodeCount = Node.NodeCount
            };
            foreach (var edge in EdgeVmList)
            {
                modelState.EdgeSaveList.Add(Mapper.Map(edge, new EdgeSave()));
            }

            for (var i = 0; i < NodeList.Count(); i++)
            {
                var nodeSave = new NodeSave();
                Mapper.Map(NodeList.ElementAt(i), nodeSave);
                Mapper.Map(NodeVmList.ElementAt(i), nodeSave);

                modelState.NodeSaveList.Add(nodeSave);
            }

            var saveDialog = new SaveFileDialog
           {
               FileName = "Document",
               DefaultExt = ".json",
               Filter = "Text documents (.json)|*.json",
               InitialDirectory = Environment.CurrentDirectory + "\\Json"
           };

            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                var fileName = saveDialog.FileName;
                _fileService.SaveFile(modelState, fileName);
            }
        }

        #endregion TextBlock Handle events

        #region Others

        private void NodeNumberValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void EdgeLambdaValidation(object sender, KeyEventArgs e)
        {
            var ch = ForGetChar.GetCharFromKey(e.Key);
            var textBox = sender as TextBox;

            if (ch == 46 && textBox.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
                return;
            }
            if (ch == 46 && textBox.Text.Length == 0)
            {
                e.Handled = true;
                return;
            }

            if (!char.IsDigit(ch) && ch != 8 && ch != 46)
            {
                e.Handled = true;
            }
        }

        private void ModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ClearViewModel();

            if (EditNodeGrid == null) return;
            EditNodeGrid.Visibility = e.Source.Equals(EditModeRadioButton) ? Visibility.Visible : Visibility.Collapsed;

            if (EditEdgeGrid == null) return;
            EditEdgeGrid.Visibility = e.Source.Equals(EditModeRadioButton) ? Visibility.Visible : Visibility.Collapsed;

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


        private void DeleteEdge(EdgeVm edgeVm)
        {
            var fromNodeVm = NodeVmList.SingleOrDefault(nv => nv.Node.Id == edgeVm.FromNodeVmId);
            var toNodeVm = NodeVmList.SingleOrDefault(nv => nv.Node.Id == edgeVm.ToNodeVmId);

            if (fromNodeVm != null)
            {
                fromNodeVm.EdgeVmList.Remove(edgeVm);
                fromNodeVm.Node.NodeRelations.RemoveAll(r => r.NodeId == edgeVm.ToNodeVmId);
            }
            if (toNodeVm != null)
            {
                toNodeVm.EdgeVmList.Remove(edgeVm);
                toNodeVm.Node.NodeRelations.RemoveAll(r => r.NodeId == edgeVm.FromNodeVmId);
            }

            EdgeVmList.Remove(edgeVm);

            WorkPlaceCanvas.Children.Remove(edgeVm.ArrowLine);


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

        private void ClearModel()
        {
            NodeList.ClearEx();
            NodeVmList.ClearEx();
            EdgeVmList.Clear();
            WorkPlaceCanvas.Children.Clear();
            ClearViewModel();
        }

        private void ClearViewModel()
        {
            NodeService.SetNodeVmColor(ref MoveNodeVm, true);
            NodeService.SetNodeVmColor(ref EditNodeVm, true);
            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
            NodeService.SetEdgeVmColor(ref EditEdgeVm, true);
        }

        private void ClearAllInputElement()
        {
            EdgeLambdaTextBox1.Clear();
            EdgeLambdaTextBox2.Clear();
            NodeNumberTextBox.Clear();
        }

        public void SetTextBlockEventHandles(TextBlock textBlock)
        {
            textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
            textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
            textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
            textBlock.MouseMove += TextBlock_MouseMove;
        }

        public void SetArrowLineEventHandles(ArrowLine arrowLine)
        {
            arrowLine.MouseLeftButtonDown += ArrowLine_MouseLeftButtonDown;
            arrowLine.MouseEnter += ArrowLine_MouseEnter;
            arrowLine.MouseLeave += ArrowLine_MouseLeave;

        }

        public bool HotKeys(object obj)
        {
            if (obj is TextBlock)
            {
                if (Keyboard.IsKeyDown(Key.D))
                {
                    var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, obj as TextBlock));
                    DeleteNode(nodeVm);
                    return true;
                }

                if (Keyboard.IsKeyDown(Key.R))
                {
                    var nodeVm = NodeVmList.Single(n => Equals(n.TextBlock, obj as TextBlock));
                    nodeVm.Node.IsRejectionNode = true;
                    return true;
                }
            }
            if (obj is ArrowLine)
            {

                if (Keyboard.IsKeyDown(Key.D))
                {
                    var edgeVm = EdgeVmList.Single(n => Equals(n.ArrowLine, obj as ArrowLine));
                    DeleteEdge(edgeVm);
                    return true;
                }
            }

            if (obj is Canvas)
            {
                return true;
            }

            return false;
        }

        #endregion Helpful methods


        #region MainWindow events handler


        #endregion


    }
}
