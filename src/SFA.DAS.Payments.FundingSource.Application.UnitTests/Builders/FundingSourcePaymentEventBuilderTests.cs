using System.Collections.Generic;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Builders;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Builders
{
    [TestFixture]
    public class FundingSourcePaymentEventBuilderTests
    {
        private long eventEmployerAccountId;
        private long transferSenderEmployerAccountId;
        private long jobId;
        private decimal sfaContributionPercentage;
        private decimal amountDue;

        private Mock<IMapper> mapper;
        private Mock<IPaymentProcessor> processor;


        private FundingSourcePaymentEventBuilder service;
        private List<FundingSourcePayment> fundingSourcePayments;


        [SetUp]
        public void SetUp()
        {
            eventEmployerAccountId = 112;
            jobId = 114;
            sfaContributionPercentage = 0.44m;
            amountDue = 3000m;
            transferSenderEmployerAccountId = 219;

            mapper = new Mock<IMapper>();
            processor = new Mock<IPaymentProcessor>();

            service = new FundingSourcePaymentEventBuilder(mapper.Object, processor.Object);
        }

        [Test]
        public void Process_BuildsFundingSourceEvents_Correctly()
        {
            var amount = new CalculatedRequiredLevyAmount
            {
                AccountId = eventEmployerAccountId,
                AmountDue = amountDue,
                AgreementId = "Agreement Two",
                SfaContributionPercentage = sfaContributionPercentage,
                TransferSenderAccountId = transferSenderEmployerAccountId
            };

            fundingSourcePayments = new List<FundingSourcePayment>
            {
                new EmployerCoInvestedPayment{ AmountDue = 540, Type = FundingSourceType.CoInvestedSfa },
                new LevyPayment{ AmountDue = 700, Type = FundingSourceType.Levy },
                new TransferPayment{ AmountDue = 900, Type = FundingSourceType.Transfer }
            };

            processor.Setup(x => x.Process(It.IsAny<RequiredPayment>()))
                .Returns(fundingSourcePayments);

            mapper.Setup(x => x.Map<FundingSourcePaymentEvent>(It.IsAny<EmployerCoInvestedPayment>())).Returns(new EmployerCoInvestedFundingSourcePaymentEvent());
            mapper.Setup(x => x.Map<FundingSourcePaymentEvent>(It.IsAny<LevyPayment>())).Returns(new LevyFundingSourcePaymentEvent());
            mapper.Setup(x => x.Map<FundingSourcePaymentEvent>(It.IsAny<TransferPayment>())).Returns(new TransferFundingSourcePaymentEvent());

            var results = service.BuildFundingSourcePaymentsForRequiredPayment(amount, transferSenderEmployerAccountId, jobId);
            processor.Verify(x=>x.Process(It.IsAny<RequiredPayment>()),Times.Once);
            processor.Verify(x => x.Process(It.Is<RequiredPayment>(rp =>
                rp.SfaContributionPercentage == sfaContributionPercentage
                && rp.AmountDue == amountDue
                && rp.IsTransfer
            )), Times.Once);

            mapper.VerifyAll();
            mapper.Verify(x => x.Map(It.IsAny<CalculatedRequiredLevyAmount>(), It.IsAny<FundingSourcePaymentEvent>()), Times.Exactly(3));

            results.Count.Should().Be(3);

            results.Should().Contain(x => x.GetType() == typeof(LevyFundingSourcePaymentEvent));
            results.Should().Contain(x => x.GetType() == typeof(EmployerCoInvestedFundingSourcePaymentEvent));
            results.Should().Contain(x => x.GetType() == typeof(TransferFundingSourcePaymentEvent));

            results.ForEach(x=>x.JobId.Should().Be(jobId));
        }

    }
}