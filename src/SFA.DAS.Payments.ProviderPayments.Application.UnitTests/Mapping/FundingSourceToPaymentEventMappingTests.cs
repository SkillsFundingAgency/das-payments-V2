using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
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
                CollectionPeriod = new CalendarPeriod {Period = 12, Month = 7, Year = 2019, Name = "1819-R12"},
                Learner = new Learner {ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1, 
                SfaContributionPercentage = 0.9m, 
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = new CalendarPeriod { Period = 12, Month = 7, Year = 2019, Name = "1819-R12" },
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
            payment.CollectionPeriodName.Should().BeEquivalentTo(employerCoInvested.CollectionPeriod.Name);
            payment.CollectionPeriodMonth.Should().Be(employerCoInvested.CollectionPeriod.Month);
            payment.CollectionPeriodYear.Should().Be(employerCoInvested.CollectionPeriod.Year);
            
            payment.DeliveryPeriodName.Should().BeEquivalentTo(employerCoInvested.CollectionPeriod.Name);
            payment.DeliveryPeriodMonth.Should().Be(employerCoInvested.CollectionPeriod.Month);
            payment.DeliveryPeriodYear.Should().Be(employerCoInvested.CollectionPeriod.Year);
            payment.DeliveryPeriod.Should().Be(employerCoInvested.CollectionPeriod.Period);
        }
    }
}