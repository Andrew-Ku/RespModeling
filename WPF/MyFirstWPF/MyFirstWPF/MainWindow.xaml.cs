using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyFirstWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Blocks = new List<TextBlock>();

            InitializeComponent();
        }

        public List<TextBlock> Blocks { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           // var el = (CreateEllipse(40, 40));

            //Grid.EditingMode=InkCanvasEditingMode.Select;
            //var s = new TextBlock()
            //{
            //    Text = "test",
            //    Height = 40,
            //    Width = 40,
            //    Padding = new Thickness(12, 10, 0, 0),
            //    Background = new VisualBrush(new Ellipse()
            //    {
            //        Height = 20,
            //        Width = 20,
            //        Fill = new SolidColorBrush(Colors.Plum)
            //    })

            //};


            //Blocks.Add(s);

            //Grid.Children.Add(s);

            //var s = button1.TransformToAncestor(Canvas);

            //Canvas.SetLeft(el, 10);
            //Canvas.SetTop(el, 20);
            //Canvas.SetBottom(el, 20);
            //Canvas.SetTop(el, 20);
        }

        Ellipse CreateEllipse(double width, double height)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            ellipse.Fill = new SolidColorBrush(Colors.SkyBlue);

            ellipse.MouseUp += Ellipse_MouseUp;
            
            return ellipse;
        }

        private void Ellipse_MouseUp(object sender, MouseEventArgs  e)
        {
            this.ShowDialog();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (var block in Blocks)
            {
                block.Margin = new Thickness(block.Margin.Left+10, block.Margin.Top+10, block.Margin.Right, block.Margin.Bottom);
                
            }
        }
        
    }

  
}
