using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.UnitTests.Helpers
{
    public class SlackAlertHelperGetEmojiTests
    {
        [TestCase("Sev0", ":alert:")]
        [TestCase("Sev1", ":alert:")]
        [TestCase("Sev2", ":warning:")]
        [TestCase("Sev3", ":information_source:")]
        [TestCase("Sev28", "")]
        public void GetEmojiSeverity0ReturnsCorrectEmojiCode(string input, string expectedOutput)
        {
            //Arrange
            var helper = new SlackAlertHelper();

            //Act
            var act = helper.GetEmoji(input);

            //Assert
            act.Should().Be(expectedOutput);
        }
    }
}