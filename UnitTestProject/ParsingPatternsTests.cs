using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using ClassroomAssignment.Model;
using System;

namespace UnitTestProject
{
    [TestClass]
    public class ParsingPatternsTests
    {
        private Match MeetingTimeMatch;

        [TestInitialize]
        public void Initialize()
        {
            MeetingTimeMatch = Regex.Match("MT 5:30pm-8:10pm", DataConstants.MeetingPatternOptions.TIME_PATTERN);
        }

        [TestMethod]
        public void Test_MeetingPatternMatches()
        {
            Assert.IsTrue(MeetingTimeMatch.Success, "Meeting Pattern does not match.");
        }

        [TestMethod]
        public void Test_PatternGroupsMatch()
        {
            // days of week
            var startTime = MeetingTimeMatch.Groups[2].Value;
            Assert.AreEqual<string>("5:30pm", startTime);

            var endTime = MeetingTimeMatch.Groups[3].Value;
            Assert.AreEqual<string>("8:10pm", endTime);
        }

        [TestMethod]
        public void TestCrossListedPattern()
        {
            var crossListingStr = "Also CSCI 8105-001, MATH 3100-001, MATH 8105-001";

            var regex = new Regex(@"\s[A-Z]+\s\d+-\d+");
            var match = regex.Matches(crossListingStr);
            var date = DateTime.Parse("07:30 PM");
        }
    }
}
