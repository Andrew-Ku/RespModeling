using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFirstWPF.Services.Interfaces;

namespace MyFirstWPF.Services
{
    public class FileTest:IFileService
    {
        public void SaveFile(string content, string path)
        {
            throw new NotImplementedException();
        }

        public void SaveFile<T>(T model, string path) where T : class
        {
            throw new NotImplementedException();
        }

        public string OpenFile(string path)
        {
            throw new NotImplementedException();
        }

        public T OpenFile<T>(string path) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
