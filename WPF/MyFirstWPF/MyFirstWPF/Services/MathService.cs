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
        public void SaveFile(ModelStateSave model)
        {
            var jsonModel = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            });

            var path = Path.Combine(Environment.CurrentDirectory, "Json");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(Path.Combine(path, "NodeJson.json"), jsonModel);
        }

        public ModelStateSave OpenFile(string path)
        {
            try
            {
                var modelStr = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<ModelStateSave>(modelStr);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
