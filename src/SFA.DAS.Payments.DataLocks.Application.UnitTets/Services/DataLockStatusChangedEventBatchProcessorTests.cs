using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Services
{
    [TestFixture]
    class DataLockStatusChangedEventBatchProcessorTests
    {
        [Test]
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("short", "short")]
        [TestCase("short-but-longer-than-9", "-longer-than-9" )]
        [TestCase("10003161/ILR-10003161-1819-20180906-161700-01-Tight.xml", "ILR-10003161-1819-20180906-161700-01-Tight.xml")]
        [TestCase("10003161/ILR-10003161-1819-20180906-161700-01-Tight-12345.xml", "ILR-10003161-1819-20180906-161700-01-Tight-12345.x")]
        public void IlrFileName_DoesNotExceed50Chars(string test, string expected)
        {
            var actual = DataLockStatusChangedEventBatchProcessor.TrimUkprnFromIlrFileNameLimitToValidLength(test);

            actual.Should().Be(expected);
        }
    }
}