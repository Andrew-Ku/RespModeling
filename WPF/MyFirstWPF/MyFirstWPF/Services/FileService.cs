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
using MyFirstWPF.Services.Interfaces;
using Newtonsoft.Json;

namespace MyFirstWPF.Services
{
    public class FileService : IFileService
    {
       /// <summary>
       /// Сохранение строки в файл по заданному пути
       /// </summary>
       /// <param name="content"></param>
       /// <param name="path"></param>
        public void SaveFile(string content,string path)
        {
            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Сохранение модели в файл по заданному пути
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        public void SaveFile<T>(T model, string path) where T : class
        {
            var jsonModel = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });

                File.WriteAllText(path, jsonModel);
        }


        public string OpenFile(string path)
        {
            try
            {
                var content = File.ReadAllText(path);
                return content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T OpenFile<T>(string path) where T : class
        {
            try
            {
                var modelStr = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(modelStr);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
