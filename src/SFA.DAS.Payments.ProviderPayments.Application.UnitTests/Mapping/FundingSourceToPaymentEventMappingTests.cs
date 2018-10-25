using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;

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
        public void Can_Map_From_Employer_CoInvested_Funding_Source_Event_To_Payment()
        {
            var employerCoInvested = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                CollectionPeriod = new CalendarPeriod {Period = 12, Month = 7, Year = 2019, Name = "1819-R12"},
                Learner = new Learner {ReferenceNumber = "1234-ref", Ukprn = 12345, Uln = 123456},
                OnProgrammeEarningType = OnProgrammeEarningType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.ContractWithSfa, 
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
                    AgreedPrice = 15000,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow
            };
            var payment = Mapper.Map<EmployerCoInvestedFundingSourcePaymentEvent, PaymentModel>(employerCoInvested);
            payment.Ukprn.Should().Be(employerCoInvested.Ukprn);
            payment.CollectionPeriod.Should().NotBeNull();
            payment.CollectionPeriod.Name.Should().BeEquivalentTo(employerCoInvested.CollectionPeriod.Name);
            payment.CollectionPeriod.Month.Should().Be(employerCoInvested.CollectionPeriod.Month);
            payment.CollectionPeriod.Year.Should().Be(employerCoInvested.CollectionPeriod.Year);
            payment.CollectionPeriod.Period.Should().Be(employerCoInvested.CollectionPeriod.Period);
        }
    }
}