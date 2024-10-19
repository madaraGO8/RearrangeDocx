using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CleanWordFile
{
    class Program
    {
        public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\New folder\PreEditingDocx\FiguresBelowReferencesOutput.docx";
        public static string newPath = Path.GetDirectoryName(path) + @"\Body.docx";
        public static bool isPresent = true;
        static void Main(string[] args)
        {
            new RearrangeDocx().RearrangeEle(path, isPresent);
            new CleanWordFile().StartBodyDocx(path, newPath, isPresent);
        }
    }
}
