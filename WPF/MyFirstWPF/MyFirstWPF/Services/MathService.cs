using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using AutoMapper;
using MyFirstWPF.Models;
using Newtonsoft.Json;
using XamlReader = System.Windows.Markup.XamlReader;
using XamlWriter = System.Windows.Markup.XamlWriter;

namespace MyFirstWPF.Services
{
    public class FileService
    {
        public void SerializeObject(NodeVm nodeVm)
        {
            nodeVm.TextBlockSave = Mapper.Map<TextBlock, TextBlockSave>(nodeVm.TextBlock);

            foreach (var edge in nodeVm.EdgeVmList)
            {
                edge.ArrowLineSave = Mapper.Map<ArrowLine, ArrowLineSave>(edge.ArrowLine);
            }

            var nodeVmCurrentSer = JsonConvert.SerializeObject(nodeVm, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });

            var path = @"C:\wpf";
            File.WriteAllText(Path.Combine(path, "Test.json"), nodeVmCurrentSer);
        }

        public void SaveFile(MainWindow mainWindow)
        {
            using (var stream = new FileStream(@"C:\Test.xaml", FileMode.Create))
            {
                XamlWriter.Save(mainWindow.Content, stream);
            }
        }

        public ContentControl OpenFile(string path = @"C:\Test.xaml")
        {
            var result = new ContentControl();
            //if (File.Exists(path))
            //{
            //    using (var stream = new FileStream(path, FileMode.Open))
            //    {
            //       result.Content = XamlReader.Load(stream);
            //    }
            //}
            var s = string.Empty;

            try
            {
                string strXaml = String.Empty;
                using (var reader = new System.IO.StreamReader(path, true))
                {
                    strXaml = reader.ReadToEnd();
                    s = strXaml;
                }

                // var xamlContent = System.Windows.Markup.XamlReader.Parse(strXaml, new ParserContext()) as Grid;
            }
            catch (System.Windows.Markup.XamlParseException ex)
            {
                // You can get specific error information like LineNumber from the exception
            }
            catch (Exception ex)
            {
                // Some other error
            }


            using (XmlReader reader = XmlReader.Create(new StringReader(path)))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(path, ws))
                {

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);
                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }
            return result;
        }
    }
}
