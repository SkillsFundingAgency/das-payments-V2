using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Tests
{
    public class FeatureInfoNumberExtractorTests
    {
        [TestCase("title-with-number-at-end-PV2-123", ExpectedResult = "123")]
        [TestCase("PV2-456-title-with-number-at-start", ExpectedResult = "456")]
        [TestCase("title-with-PV2-789-in-middle", ExpectedResult = "789")]
        [TestCase("title-with-large-PV2-123456789", ExpectedResult = "123456789")]
        [TestCase("title-with-small-PV2-1", ExpectedResult = "1")]
        [TestCase("lowercase-pv2-111", ExpectedResult = "111")]
        [TestCase("mixedcase-Pv2-222", ExpectedResult = "222")]
        [TestCase("mixedcase-pV2-333", ExpectedResult = "333")]
        [TestCase("underscore-pV2_444", ExpectedResult = "444")]
        public string ExtractPvNumberFromTitle(string title) => FeatureNumber.ExtractFrom(title);

        [TestCase("title-without")]
        [TestCase("title-with-no-number-PV2")]
        [TestCase("title-with-no-number-PV2-")]
        [TestCase("")]
        [TestCase(null)]
        public void ExtractPvNumberFromTitleWithout(string title)
        {
            Assert.That(() => FeatureNumber.ExtractFrom(title), Throws.ArgumentException.With.Message.Contains("does not contain a PV2 number"));
        }
    }
}
