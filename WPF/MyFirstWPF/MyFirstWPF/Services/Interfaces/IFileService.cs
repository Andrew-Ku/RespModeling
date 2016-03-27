using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFirstWPF.Services.Interfaces
{
    public interface IFileService
    {
        void SaveFile(string content, string path);
        void SaveFile<T>(T model, string path) where T : class;
        string OpenFile(string path);
        T OpenFile<T>(string path) where T : class;
    }
}
