using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace ReadExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            var myDictionary = new Dictionary<string, int>();
            myDictionary.Add("WI", 0);
            myDictionary.Add("H1", 1);
            myDictionary.Add("H2", 2);
            myDictionary.Add("H3", 3);
            myDictionary.Add("H4", 4);
            myDictionary.Add("H5", 5);
            myDictionary.Add("L1", 6);
            myDictionary.Add("L2", 7);
            myDictionary.Add("L3", 8);
            myDictionary.Add("L4", 9);
            myDictionary.Add("L5", 10);
            myDictionary.Add("L6", 11);
            myDictionary.Add("BN", 13);



            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\liorh\Copy of Book1");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 1; i <= colCount; i++)
            {
                string total = "";
                for (int j = 1; j <= rowCount; j++)
                {
                    //new line
                    if (j == 1)
                        Console.Write("\r\n");

                    //write the value to the console
                    if (xlRange.Cells[j, i] != null && xlRange.Cells[j, i].Value2 != null)
                    {
                        string key = xlRange.Cells[j, i].Value2.ToString();
                        total += myDictionary[key] + ", ";
                    }
                }
            }
        }
    }
}
