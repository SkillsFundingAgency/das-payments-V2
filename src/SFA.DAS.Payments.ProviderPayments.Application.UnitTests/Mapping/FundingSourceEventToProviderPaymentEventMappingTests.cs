using System;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class FundingSourceEventToProviderPaymentEventMappingTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<ProviderPaymentsProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }
        
        [TestCase(typeof(EmployerCoInvestedFundingSourcePaymentEvent), typeof(EmployerCoInvestedProviderPaymentEvent))]
        [TestCase(typeof(SfaCoInvestedFundingSourcePaymentEvent), typeof(SfaCoInvestedProviderPaymentEvent))]
        [TestCase(typeof(SfaFullyFundedFundingSourcePaymentEvent), typeof(SfaFullyFundedProviderPaymentEvent))]
        [TestCase(typeof(LevyFundingSourcePaymentEvent), typeof(LevyProviderPaymentEvent))]
        public void MapsFromFundingSourceEventToCorrectProviderPaymentEvent(Type sourceType, Type destType)
        {
            var fundingSourceEvent = Activator.CreateInstance(sourceType) as FundingSourcePaymentEvent;
            fundingSourceEvent.EventId = Guid.NewGuid();
            fundingSourceEvent.CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 };
            fundingSourceEvent.Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 };
            fundingSourceEvent.TransactionType = TransactionType.Completion;
            fundingSourceEvent.Ukprn = 12345;
            fundingSourceEvent.ContractType = ContractType.Act1;
            fundingSourceEvent.SfaContributionPercentage = 0.9m;
            fundingSourceEvent.PriceEpisodeIdentifier = "pe-1";
            fundingSourceEvent.JobId = 123;
            fundingSourceEvent.AmountDue = 300;
            fundingSourceEvent.FundingSourceType = FundingSourceType.CoInvestedEmployer;
            fundingSourceEvent.DeliveryPeriod = 12;
            fundingSourceEvent.LearningAim = new LearningAim
            {
                PathwayCode = 12,
                FrameworkCode = 1245,
                FundingLineType = "Non-DAS 16-18 Learner",
                StandardCode = 1209,
                ProgrammeType = 7890,
                Reference = "1234567-aim-ref"
            };
            fundingSourceEvent.IlrSubmissionDateTime = DateTime.UtcNow;
            fundingSourceEvent.EventTime = DateTimeOffset.UtcNow;
            fundingSourceEvent.RequiredPaymentEventId = Guid.NewGuid();
            fundingSourceEvent.AccountId = 123456789;

            var payment = Mapper.Map<ProviderPaymentEvent>(fundingSourceEvent);
            payment.Should().NotBeNull();
            payment.Should().BeAssignableTo(destType);
        }
    }
}