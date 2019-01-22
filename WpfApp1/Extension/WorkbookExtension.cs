using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Extension
{
    static class WorkbookExtension
    {
        /// <summary>
        /// Sort worksheets by their appearance 
        /// </summary>
        /// <param name="workbook"></param>       
        public static void SortWorksheets(this IWorkbook workbook) 
        {
            var listOfNames = new List<string>();

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                listOfNames.Add(workbook.GetSheetName(i));
            }

            listOfNames.Sort();

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                workbook.SetSheetOrder(listOfNames[i], i);
            }
        }
        /// <summary>
        /// Open outputFileName and write
        /// to the workbook
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="outputFileName"></param>
        public static void WriteToFile(this IWorkbook workbook, string outputFileName)
        {
            using (var fileStream = File.OpenWrite(outputFileName))
            {
                workbook.Write(fileStream);
            }
        }
    }
}
