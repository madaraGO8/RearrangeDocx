using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanWordFile
{
    class Program
    {
        public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\New folder\PreEditingDocx\RASP_2024_2363936_2024-05-31_23-29-12.docx";
        public static string newPath = Path.GetDirectoryName(path) + @"\Body.docx";
        public static string preEditingPath = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\New folder\PreEditingDocx\FNAS_2024_2376555_2024-07-02_22-53-15.docx";
        public static bool isPresent = true;
        static void Main(string[] args)
        {
            new CleanWordFile().CleanDocx(path, newPath, isPresent);
            new CleanWordFile().RemoveEmptyParagraphs(newPath, isPresent);
            //new RearrangeDocx().RearrangeEle(preEditingPath, isPresent);
        }
    }
}
