using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private readonly string JsonFilesPath;

        public List<NodeVm> NodeVmList;
        public List<EdgeVm> EdgeVmList;
        public List<Node> NodeList;
        public double NodeRadius;
        public NodeVm StartNodeVmEdge;
        public bool MoveFlag;
        public Point CurrPosition;
        public NodeVm SelectNodeVm;
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
            JsonFilesPath = Path.Combine(Environment.CurrentDirectory, "Json");

            Properties.Settings.Default.LastSaveFile = "";
            Properties.Settings.Default.LastOpenFile = "";
            Properties.Settings.Default.Save();

            InitializeComponent();
        }

        #endregion Initialization

        #region WorkPlaceCanvas Handle events
        private void WorkPlaceCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearViewModel();
            // ClearAllInputElement();

            //if (Keyboard.IsKeyDown(Key.D))
            //{
            //    e.Handled = true;
            //    return;
            //}

            if (HotKeys(sender))
            {
                e.Handled = true;
                return;
            }

            //  if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
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
                UpdateModelInfo();
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
            //if (e.Delta < 0)
            //    EditModeRadioButton.IsChecked = true;
            //else
            //    CreateModeRadioButton.IsChecked = true;
        }

        #endregion WorkPlaceCanvas Handle events

        #region TextBlock Handle events

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            NodeService.SetNodeVmColor(ref StartNodeVmEdge);

            #region Создание связи

            //  if (CreateModeRadioButton.IsChecked.GetValueOrDefault())
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
                    SelectNodeVm = nodeVm;
                    FillNodeEditFields(ref SelectNodeVm);
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


                        NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
                        UpdateModelInfo();
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

                    StartNodeVmEdge.EdgeVmList.Add(edgeVm);
                    nodeVm.EdgeVmList.Add(edgeVm);

                    EdgeVmList.Add(edgeVm);
                    WorkPlaceCanvas.Children.Add(edgeVm.ArrowLine);
                    NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
                    UpdateModelInfo();
                    FillDataGrid();
                }

            }

            #endregion
            e.Handled = true;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
            NodeService.SetNodeVmColor(ref SelectNodeVm);
            NodeService.SetEdgeVmColor(ref EditEdgeVm, true);

            if (HotKeys(sender))
            {
                e.Handled = true;
                return;
            }

            //   if (CreateModeRadioButton.IsChecked == true)
            {
                CurrPosition = e.GetPosition(WorkPlaceCanvas);
                MoveFlag = true;
                SelectNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                NodeService.SetNodeVmColor(ref SelectNodeVm, border: NodeColors.MoveNodeBorder);
                Mouse.Capture(SelectNodeVm.TextBlock);
            }
            //  if (EditModeRadioButton.IsChecked == true)
            {
                //  SelectNodeVm = NodeVmList.Single(n => Equals(n.TextBlock, sender));
                NodeService.SetNodeVmColor(ref SelectNodeVm, border: NodeColors.EditNodeBorder);

                FillNodeEditFields(ref SelectNodeVm);

                FillDataGrid();
            }

            e.Handled = true;
        }
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectNodeVm == null) return;
            Mouse.Capture(null);
            MoveFlag = false;
            NodeService.SetNodeVmColor(ref SelectNodeVm, border: NodeColors.NormalBorder);
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

                SelectNodeVm.TextBlock.Margin = new Thickness(cursorPosition.X - NodeRadius, cursorPosition.Y - NodeRadius, 0, 0);
                SelectNodeVm.Position = new Point(cursorPosition.X, cursorPosition.Y);
            }

            e.Handled = true;
        }

        #endregion TextBlock Handle events

        #region ArrowLine Hande events

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

            if (!NodeList.Single(n=>n.IsStartNode).NodeRelations.Any())
            {
                MessageBox.Show("Переход из стартового узла обязателен");
                return;
            }


            var relationNodes = NodeList.SelectMany(n => n.NodeRelations).Select(r => r.NodeId).ToList();
            if (NodeList.Any(node => !relationNodes.Contains(node.Id) && !node.NodeRelations.Any()))
            {
                MessageBox.Show("Все состояния должны иметь связи");
                return;
            }

            var genWindow = new StartGpssWindow(NodeList)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            genWindow.ShowDialog();

        }

        private void NewButton_OnClick(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LastSaveFile = "";
            Properties.Settings.Default.Save();
            ClearModel();
        }

        private void SaveEditNodeButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectNodeVm == null) return;

            var regex = new Regex("[^0-9]+");
            if (regex.IsMatch(NodeNumberTextBox.Text))
            {
                MessageBox.Show("Некорректный номер узла");
                return;
            }

            if (NodeNumberTextBox.Text.Length == 2 && NodeNumberTextBox.Text.StartsWith("0"))
            {
                NodeNumberTextBox.Text = NodeNumberTextBox.Text.Substring(1);
            }

            var number = int.Parse(NodeNumberTextBox.Text);

            if (NodeList.Any(n => n.Id == number) && number != SelectNodeVm.Node.Id)
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
                var newStartNodeVm = NodeVmList.Single(nv => nv.Equals(SelectNodeVm));
                newStartNodeVm.TextBlock.Background = new VisualBrush(NodeService.GetEllipse(NodeRadius, NodeColors.StartNodeBackground, NodeColors.NormalBorder));

            }

            ChangeNodeId(SelectNodeVm.Node.Id, number);
            SelectNodeVm.Node.IsStartNode = StartNodeCheckBox.IsChecked.GetValueOrDefault();
            SelectNodeVm.Node.IsRejectionNode = RejectionNodeCheckBox.IsChecked.GetValueOrDefault();

            NodeNumberTextBox.Clear();
            NodeService.SetNodeVmColor(ref SelectNodeVm, border: NodeColors.NormalBorder);
        }

        private void OpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            var lastOpenFile = Properties.Settings.Default.LastOpenFile;

            if (!File.Exists(Path.Combine(JsonFilesPath, lastOpenFile)))
            {
                lastOpenFile = "";
                Properties.Settings.Default.LastOpenFile = "";
                Properties.Settings.Default.Save();
            }

            var fileDialog = new OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = " (.json)|*.json",
                InitialDirectory = JsonFilesPath,
                FileName = lastOpenFile != string.Empty ? lastOpenFile : ""
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

                    EdgeVmList.Add(edgeVm);
                    WorkPlaceCanvas.Children.Add(edgeVm.ArrowLine);
                }

                Properties.Settings.Default.LastOpenFile = fileDialog.SafeFileName;
                Properties.Settings.Default.LastSaveFile = fileDialog.SafeFileName;
                Properties.Settings.Default.Save();

                this.Title = fileDialog.FileName;
                UpdateModelInfo();
                FillDataGrid();

            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!NodeList.Any()) return;

            var lastSaveFile = Properties.Settings.Default.LastSaveFile;

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
               FileName = lastSaveFile,
               DefaultExt = ".json",
               Filter = "Text documents (.json)|*.json",
               InitialDirectory = JsonFilesPath,
               OverwritePrompt = false
           };

            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                var fileName = saveDialog.FileName;
                Properties.Settings.Default.LastSaveFile = saveDialog.SafeFileName;
                Properties.Settings.Default.Save();
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
            var allowButton = new List<Key>() { Key.Left, Key.Right, Key.LeftCtrl, Key.RightCtrl, Key.V, Key.Delete };


            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.V))
            {

                var pasteText = Clipboard.GetText().Replace(",", ".");
                var regexChar = new Regex("[^0-9\\.]");
                var regexPoint = new Regex("\\.");

                if (!regexChar.IsMatch(pasteText) && regexPoint.Matches(pasteText).Count < 2 && pasteText.First() != '.' && pasteText.Last() != '.')
                {
                    textBox.Text = pasteText;
                    textBox.CaretIndex = pasteText.Length;
                    e.Handled = true;
                    return;
                }

                e.Handled = true;
                return;
            }

            if (ch == 46 && (textBox.Text.IndexOf('.') != -1
                || textBox.CaretIndex == 0
                || textBox.Text.Length == 0)
                )
            {
                e.Handled = true;
                return;
            }

            if (!char.IsDigit(ch) && ch != 8 && ch != 46 && !allowButton.Contains(e.Key))
            {
                e.Handled = true;
            }

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
            UpdateModelInfo();

            if (nodeVm.Equals(SelectNodeVm))
            {
                NodeNumberTextBox.Clear();
                NodeService.SetNodeVmColor(ref SelectNodeVm, true);
                FillDataGrid();
            }


            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
        }


        // Удал ребра
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
            UpdateModelInfo();

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
            SelectNodeVm.Node.Id = newId;

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
            UpdateModelInfo();
        }

        private void ClearViewModel()
        {
            NodeService.SetNodeVmColor(ref StartNodeVmEdge, true);
            NodeService.SetEdgeVmColor(ref EditEdgeVm, true);
        }

        public void SetTextBlockEventHandles(TextBlock textBlock)
        {
            textBlock.MouseRightButtonDown += TextBlock_MouseRightButtonDown;
            textBlock.MouseLeftButtonDown += TextBlock_MouseLeftButtonDown;
            textBlock.MouseLeftButtonUp += TextBlock_MouseLeftButtonUp;
            textBlock.MouseMove += TextBlock_MouseMove;
        }


        // Работа с горячими клавишами
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
                    if (nodeVm.Node.IsStartNode) return false;

                    nodeVm.Node.IsRejectionNode = !nodeVm.Node.IsRejectionNode;
                    NodeService.SetNodeVmColor(ref nodeVm);
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
                if (Keyboard.IsKeyDown(Key.D))
                {
                    return true;
                }
            }

            return false;
        }

        // Обновление  вспомогательной информации
        private void UpdateModelInfo()
        {
            StateCountLabel.Content = NodeList.Count();
            var edgeCount = 0;
            foreach (var edgeVm in EdgeVmList)
            {
                if (edgeVm.ArrowLine.ArrowEnds == ArrowEnds.Both)
                {
                    edgeCount += 2;
                }
                else
                {
                    edgeCount++;
                }
            }

            EdgeCountLabel.Content = edgeCount;
        }

        // Заполнение грида свзяей
        public void FillDataGrid()
        {          
            NodeRelationDataGrid.CancelEdit();

            if (SelectNodeVm == null)
            {
                NodeRelationDataGrid.ItemsSource = null;
                NodeRelationDataGrid.Items.Refresh();
                return;
            }

            NodeRelationDataGrid.ItemsSource = SelectNodeVm.Node.NodeRelations;
            NodeRelationDataGrid.Items.Refresh();
        }

        // Заполнение блока редактирования состояния
        public void FillNodeEditFields(ref NodeVm nodeVm)
        {
            NodeNumberTextBox.Text = nodeVm.Node.Id.ToString();
            StartNodeCheckBox.IsChecked = nodeVm.Node.IsStartNode;
            RejectionNodeCheckBox.IsChecked = nodeVm.Node.IsRejectionNode;

            if (nodeVm.Node.IsStartNode)
            {
                StartNodeCheckBox.IsEnabled = false;
                RejectionNodeCheckBox.IsEnabled = false;
            }
            else
            {
                StartNodeCheckBox.IsEnabled = true;
                RejectionNodeCheckBox.IsEnabled = true;
            }
            FillDataGrid();
        }

        #endregion Helpful methods


        #region MainWindow events handler


        #endregion


        #region NodeRelationDataGrid events handler

        // Обработчик для кнопки delete. Удаление визуальных связей при удаление связей из грида 
        private void NodeRelationDataGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var deleteRelationIds = NodeRelationDataGrid.SelectedItems.OfType<NodeRelation>().Select(r => r.NodeId).ToList();

                foreach (var id in deleteRelationIds)
                {
                    var edge = EdgeVmList.Single(c => c.FromNodeVm.Node.Id == SelectNodeVm.Node.Id && c.ToNodeVm.Node.Id == id || c.ToNodeVm.Node.Id == SelectNodeVm.Node.Id && c.FromNodeVm.Node.Id == id);
                    if (edge.ArrowLine.ArrowEnds == ArrowEnds.Both)
                    {
                        edge.ArrowLine.ArrowEnds = NodeService.VectorLen(SelectNodeVm.Position, new Point(edge.ArrowLine.X1, edge.ArrowLine.Y1)) >
                                                   NodeService.VectorLen(SelectNodeVm.Position, new Point(edge.ArrowLine.X2, edge.ArrowLine.Y2)) ? ArrowEnds.End : ArrowEnds.Start;
                    }
                    else
                    {
                        DeleteEdge(edge);
                    }
                }
            }
        }

        #endregion

    }
}
