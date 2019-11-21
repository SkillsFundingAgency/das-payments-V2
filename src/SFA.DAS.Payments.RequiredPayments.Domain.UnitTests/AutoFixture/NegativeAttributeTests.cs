using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.Payments.RequiredPayments.Domain.UnitTests.AutoFixture
{
    class NegativeAttributeTests
    {
        [Test, AutoData]
        public void Negatives([Negative] decimal neg)
        {
            neg.Should().BeNegative();
        }
    }
}