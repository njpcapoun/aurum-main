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
    /// Sets template for exported schedule 
    /// visualization
    /// </summary>
    public class ClassScheduleTemplate
    {
        public const string SCHEDULE_TEMPLATE_NAME = "ScheduleTemplate";
        private List<short> AvailableColors;
        private int CurrentColorIndex = 0;
        private Dictionary<string, short> SubjectCodeToColor = new Dictionary<string, short>();

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
        /// Sets the style of cells for the workbook
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="foregroundColor"></param>
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

        

        public Dictionary<string, short> GetSubjectColorMap()
        {
            return SubjectCodeToColor;
        }
    }
}
