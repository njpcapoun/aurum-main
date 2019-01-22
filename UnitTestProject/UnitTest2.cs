using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class UITest
    {

        [TestMethod]
        public void TestMethod1()
        {
            //var excelPath = ConfigurationManager.AppSettings["Excel_Path"];
            //Process.Start("EXCEL.EXE");

            //var count = 0;
            //AutomationElement excelAutomationElement = null;
            //do
            //{
            //    excelAutomationElement = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Excel"));

            //    count++;
            //    Thread.Sleep(100);
            //} while (excelAutomationElement == null && count < 50);

            //if (excelAutomationElement == null)
            //{
            //    Assert.Fail("Failed to start Excel");
            //}

            //var button = excelAutomationElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Blank workbook"));
            //InvokePattern invokePattern = (InvokePattern) button.GetCurrentPattern(InvokePattern.Pattern);
            //invokePattern.Invoke();
        }
    }
}
