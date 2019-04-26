using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace ClassroomAssignment.Model
{
    /// <summary>
    /// Sets template and visualization for the exported schedules.
    /// </summary>
    public class ClassScheduleTemplate
    {
        public const string SCHEDULE_TEMPLATE_NAME = "ScheduleTemplate";
        private List<short> AvailableColors;
        private int CurrentColorIndex = 0;
        private Dictionary<string, short> SubjectCodeToColor = new Dictionary<string, short>();

		/// <summary>
		/// Constructor for ClassScheduleTemplate. Set the list of colors for the subject codes.
		/// </summary>
        public ClassScheduleTemplate()
        {
            AvailableColors = new List<short>()
            {
                IndexedColors.Coral.Index,
                IndexedColors.LightGreen.Index,
                IndexedColors.LightBlue.Index,
                IndexedColors.LightOrange.Index,
                IndexedColors.LightYellow.Index,
                IndexedColors.Lime.Index,
                IndexedColors.Rose.Index,
                IndexedColors.Teal.Index,
            };
        }

       

        /// <summary>
        /// Sets the style of cells for the workbook.
        /// </summary>
        /// <param name="workbook">An Excel workbook.</param>
        /// <param name="foregroundColor">The foreground color based on subject code.</param>
        /// <returns>ICellStyle</returns>
        public ICellStyle GetCellStyle(IWorkbook workbook, string subjectCode)
        {
            short foregroundColor = GetColorForSubjectCode(subjectCode);
            IFont font = workbook.CreateFont();
            font.Boldweight = 550;
            font.FontName = "Calibri";
            font.FontHeightInPoints = 11;

            ICellStyle style = workbook.CreateCellStyle();
            style.SetFont(font);
            style.WrapText = true;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = HorizontalAlignment.Center;
            style.FillForegroundColor = foregroundColor;
            style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

		/// <summary>
		/// Getter for department color.
		/// </summary>
		/// <param name="subjectCode">Subject code of a course.</param>
		/// <returns></returns>
        private short GetColorForSubjectCode(string subjectCode)
        {
            if (SubjectCodeToColor.ContainsKey(subjectCode))
            {
                return SubjectCodeToColor[subjectCode];
            }
            else
            {
                if (CurrentColorIndex < AvailableColors.Count)
                {
                    short color = AvailableColors[CurrentColorIndex++];
                    SubjectCodeToColor[subjectCode] = color;
                    return color;
                }

                return AvailableColors.Last();
            }
        }
        
		/// <summary>
		/// Getter for subject code color.
		/// </summary>
		/// <returns></returns>
        public Dictionary<string, short> GetSubjectColorMap()
        {
            return SubjectCodeToColor;
        }
    }
}
