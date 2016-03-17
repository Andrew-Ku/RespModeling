using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using MyFirstWPF.Models;
using MyFirstWPF.Models.Generation;
using MyFirstWPF.Services;
using Path = System.IO.Path;

namespace MyFirstWPF
{
    /// <summary>
    /// Interaction logic for StartGpssWindow.xaml
    /// </summary>
    public partial class StartGpssWindow : Window
    {
        private readonly List<Node> NodeList;
        private readonly string GpssFilesPath;

        public StartGpssWindow(List<Node> nodeList)
        {
            NodeList = nodeList;
            GpssFilesPath = Path.Combine(Environment.CurrentDirectory, "GPSS");
            InitializeComponent();

            ModelingTimeTextBox.Focus();
        }

        private void StartGenButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ModelingTimeTextBox.Text.Length == 0 || ObservationTimeTextBox.Text.Length == 0)
            {
                MessageBox.Show("Все параметры должны быть заданы");
                return;
            }

            if (ModelingTimeTextBox.Text.All(s => s == '0') || ObservationTimeTextBox.Text.All(s => s == '0'))
            {
                MessageBox.Show("Все параметры должны быть заданы");
                return;
            }

            var modelingTime = int.Parse(string.Concat(ModelingTimeTextBox.Text.SkipWhile(s => s == '0')));
            var observationTime = int.Parse(string.Concat(ObservationTimeTextBox.Text.SkipWhile(s => s == '0')));

            if (modelingTime < observationTime)
            {
                MessageBox.Show("Время моделирования должно быть больше времени наблюдения");
                return;
            }

            if (!Directory.Exists(GpssFilesPath))
            {
                Directory.CreateDirectory(GpssFilesPath);
            }

            var lastSaveFile = Properties.Settings.Default.LastSaveFile.Replace(".json", ".txt");

            var saveDialog = new SaveFileDialog
         {
             DefaultExt = ".txt",
             Filter = "Text documents (.txt)|*.txt",
             InitialDirectory = GpssFilesPath,
             FileName = lastSaveFile,
             OverwritePrompt = false
         };

            var result = saveDialog.ShowDialog();

            if (result == true)
            {
                var fileName = saveDialog.FileName;
                GenerateService.GenerateGpssFile(new GpssInputModel
                {
                    Nodes = NodeList,
                    ModelingTime = modelingTime,
                    ObservationTime = observationTime
                }, fileName);
            }
            this.Close();

        }

        private void CancelGenButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void IntNumberValidation(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void StartGpssWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                  this.Close();
            }

            if (e.Key == Key.Enter)
            {
                StartGenButton_OnClick(null, null);
            }
               
        }
    }
}
