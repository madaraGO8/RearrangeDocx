using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlDataExtraction
{
    class Program
    {
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\BJOS_2023_13094_2024-04-02_10-39-36 4\bjos-3181-metadata.xml";
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\EQE_2023_4083_2024-01-02_04-33-40 4\eqe-23-0221-metadata.xml";
        public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\CTD2_2023_262_2024-01-02_03-21-40 2\ctd2-2023-12-0138-metadata.xml";
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\CMUE_2024_2365717_2024-06-06_00-17-22 3\cmue-2023-0136-20240605201030\cmue-2023-0136-metadata.xml";
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\CMUE_2024_2365700_2024-07-31_00-17-22\cmue-2023-0136-20240605201030\cmue-2023-0136-metadata.xml";
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\CJOL_2024_2374260_2024-06-26_23-09-15 5\cjol-2023-0328-20240626190436\cjol-2023-0328-metadata.xml";
        //public static string path = @"C:\Users\Prathamesh.sulakhe\Desktop\Folders\Packages\02-09-2024\10Packages\New folder\CJAS_2024_2373939_2024-06-26_01-42-18\cjas-2023-0605-20240625213236\cjas-2023-0605-metadata.xml";
        static void Main(string[] args)
        {
           new ReadMetaData().ReadMetaDataXml(path);
        }
    }
}
