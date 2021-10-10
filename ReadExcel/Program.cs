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
            myDictionary.Add("WW", 0);
            myDictionary.Add("H1", 1);
            myDictionary.Add("H2", 2);
            myDictionary.Add("M1", 3);
            myDictionary.Add("M2", 4);
            myDictionary.Add("L1", 5);
            myDictionary.Add("L2", 6);
            myDictionary.Add("L3", 7);
            myDictionary.Add("L4", 8);
            myDictionary.Add("JP", 9);
            myDictionary.Add("JP1", 10);
            myDictionary.Add("JP2", 11);
            myDictionary.Add("JP3", 12);
            myDictionary.Add("JP4", 13);
            myDictionary.Add("BN", 14);



            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\liorh\Copy Of Mermaids");
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;
            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            string total = "";
            string totalWeights = "";
            for (int i = 1; i <= colCount; i++)
            {
                if (i % 2 != 0)
                {
                    total += "[";
                }
                if (i % 2 == 0)
                {
                    totalWeights += "[";
                }
                for (int j = 1; j <= rowCount; j++)
                {
                    //new line
                    if (j == 1)
                        Console.Write("\r\n");
                    string key = null;
                    var z = xlRange.Cells[j, i];
                    if (xlRange.Cells[j, i] != null && xlRange.Cells[j, i].Value2 != null)
                    {
                        key = xlRange.Cells[j, i].Value2.ToString();
                        if (key == "" || key == null)
                        {
                            continue;
                        }
                    }

                    //write the value to the console
                    if (i % 2 != 0 && xlRange.Cells[j, i] != null && xlRange.Cells[j, i].Value2 != null)
                    {
                        total += myDictionary[key] + ", ";
                    }

                    if (i % 2 == 0 && xlRange.Cells[j, i] != null && xlRange.Cells[j, i].Value2 != null)
                    {
                        if (key == null || key == "" || key == " ")
                        {

                        }

                        if (key.Contains("E"))
                        {

                        }
                        totalWeights += key + ", ";
                    }
                }

                if (i % 2 != 0)
                {
                    total = total.Remove(total.Length - 2);
                    total += "]";
                    total += "\n";
                }
                if (i % 2 == 0)
                {
                    totalWeights = totalWeights.Remove(totalWeights.Length - 2);
                    totalWeights += "]";
                    totalWeights += "\n";
                }
            }
        }
    }
}
