using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClassroomAssignment.Extension
{
	/// <summary>
	/// Extension methods for the workbook interface.
	/// </summary>
    static class WorkbookExtension
    {
        /// <summary>
        /// Sort worksheets by their appearance.
        /// </summary>
        /// <param name="workbook">A Microsoft Excel workbook.</param>       
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
        /// Open outputFileName and write to the workbook.
        /// </summary>
        /// <param name="workbook">A Microsoft Excel workbook.</param>
        /// <param name="outputFileName">The location of the file being written to.</param>
        public static void WriteToFile(this IWorkbook workbook, string outputFileName)
        {
			try
			{
				using (var fileStream = File.OpenWrite(outputFileName))
				{
					workbook.Write(fileStream);
				}
			}
			catch (IOException e)
			{
				Console.WriteLine("Exception: " + e);
				MessageBoxResult messageBoxResult = MessageBox.Show("Unable to save to "+ outputFileName + ".\nPlease close the file and try again.", "Save As Unsuccssful", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
    }
}
