using System;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class LevyMessageRoutingServiceTests
    {
        private AutoMock mocker;

        [SetUp]
        public void SetUp()
        {
            mocker =  AutoMock.GetLoose();
        }

        [Test]
        public void Uses_Sender_Account_Id_For_Transfers()
        {
            var service = mocker.Create<LevyMessageRoutingService>();
            var accountId = service.GetDestinationAccountId(new CalculatedRequiredLevyAmount
                {AccountId = 1, TransferSenderAccountId = 99});
            accountId.Should().Be(99);
        }

        [Test]
        public void Uses_Receiver_Account_Id_For_Non_Transfers()
        {
            var service = mocker.Create<LevyMessageRoutingService>();
            var accountId = service.GetDestinationAccountId(new CalculatedRequiredLevyAmount
                { AccountId = 1, TransferSenderAccountId = 1 });
            accountId.Should().Be(1);
        }

        [Test]
        public void Throws_If_Account_Id_Is_Null()
        {
            var service = mocker.Create<LevyMessageRoutingService>();
            Assert.Throws<InvalidOperationException>(() => service.GetDestinationAccountId(new CalculatedRequiredLevyAmount
                { AccountId = null, TransferSenderAccountId = 1 }));
        }
    }
}