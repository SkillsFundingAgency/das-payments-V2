using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class FundingSourceToPaymentEventMappingTests
    {
        
        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ProviderPaymentsProfile>();
            });
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void CanMapFromEmployerCoInvestedFundingSourceEventToPayment()
        {
            var employerCoInvested = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                CollectionPeriod = new CollectionPeriod {Period = 12, AcademicYear = 1819},
                Learner = new Learner {ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1, 
                SfaContributionPercentage = 0.9m, 
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow
            };
            var payment = Mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent, ProviderPaymentEventModel>(employerCoInvested);
            payment.Ukprn.Should().Be(employerCoInvested.Ukprn);
            payment.CollectionPeriod.Should().Be(employerCoInvested.CollectionPeriod.Period);
            payment.AcademicYear.Should().Be(employerCoInvested.CollectionPeriod.AcademicYear);
            payment.DeliveryPeriod.Should().Be(employerCoInvested.DeliveryPeriod);
            payment.FundingSourceId.Should().Be(employerCoInvested.EventId);
            payment.ContractType.Should().Be(employerCoInvested.ContractType);
            payment.SfaContributionPercentage.Should().Be(employerCoInvested.SfaContributionPercentage);
            payment.Amount.Should().Be(employerCoInvested.AmountDue);
            payment.FundingSource.Should().Be(employerCoInvested.FundingSourceType);
            payment.JobId.Should().Be(employerCoInvested.JobId);
            payment.IlrSubmissionDateTime.Should().Be(employerCoInvested.IlrSubmissionDateTime);
        }

        [Test]
        public void CanMapFromLevyFundingSourcePaymentEventToPayment()
        {
            var agreementId = "AGREEMENT";

            var levy = new LevyFundingSourcePaymentEvent
            {
                CollectionPeriod = new CollectionPeriod {Period = 12, AcademicYear = 1819, Name = "1819-R12"},
                Learner = new Learner {ReferenceNumber = "1234-ref", Uln = 123456},
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
                AgreementId = agreementId
            };

            var payment = Mapper.Map<LevyFundingSourcePaymentEvent, PaymentModel>(levy);
            payment.Ukprn.Should().Be(levy.Ukprn);
            payment.CollectionPeriod.Should().NotBeNull();
            payment.CollectionPeriod.Name.Should().BeEquivalentTo(levy.CollectionPeriod.Name);
            payment.CollectionPeriod.Period.Should().Be(levy.CollectionPeriod.Period);
        }
    }
}