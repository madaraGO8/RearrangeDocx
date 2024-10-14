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
        public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\New folder\PreEditingDocx\CTAS_2024_2414825_2024-10-07_23-27-40.docx";
        public static string newPath = Path.GetDirectoryName(path) + @"\Body.docx";
        public static string preEditingPath = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\New folder\PreEditingDocx\CJMS_2024_2363850_2024-05-31_22-56-07.docx";
        public static bool isPresent = true;
        static void Main(string[] args)
        {
            new RearrangeDocx().RearrangeEle(path, isPresent);
            List<XElement> EndOrFootNotes = new CleanWordFile().CleanDocx(path, newPath, isPresent);
            new CleanWordFile().RemoveEmptyParagraphs(newPath, isPresent);
            new CleanWordFile().RemoveFootnotesEndnotes(newPath, EndOrFootNotes, isPresent);
            new CleanWordFile().RemoveSectionBreaks(newPath);
        }
    }
}
