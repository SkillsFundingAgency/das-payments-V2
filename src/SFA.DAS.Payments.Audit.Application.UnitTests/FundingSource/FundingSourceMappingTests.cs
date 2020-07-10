using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.FundingSource;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.FundingSource
{
    public abstract class FundingSourceMappingTests<TSource>
        where TSource : FundingSourcePaymentEvent
    {
        protected IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            var config = new MapperConfiguration(AddProfile);
            Mapper = new Mapper(config);
        }

        protected TSource PaymentEvent { get; set; }
        protected abstract TSource CreatePaymentEvent();

        [SetUp]
        public void SetUp()
        {
            PaymentEvent = CreatePaymentEvent();
            PopulateCommonProperties(PaymentEvent);
        }

        protected virtual void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<FundingSourceProfile>();
        }

        [Test]
        public void Maps_SfaContributionPercentage()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).SfaContributionPercentage.Should().Be(PaymentEvent.SfaContributionPercentage);
        }

        [TestCaseSource(nameof(GetContractTypes))]
        public void Maps_ContractType(ContractType contractType)
        {
            PaymentEvent.ContractType = contractType;
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).ContractType.Should().Be(PaymentEvent.ContractType);
        }

        [TestCaseSource(nameof(GetTransactionTypes))]
        public void Maps_TransactionType(TransactionType transactionType)
        {
            PaymentEvent.TransactionType = transactionType;
            var model = Mapper.Map<FundingSourceEventModel>(PaymentEvent);
            model.TransactionType.Should().Be(PaymentEvent.TransactionType);
        }

        public static Array GetTransactionTypes()
        {
            return GetEnumValues<TransactionType>();
        }

        [TestCaseSource(nameof(GetFundingSource))]
        public void Maps_FundingSource(FundingSourceType fundingSource)
        {
            PaymentEvent.FundingSourceType = fundingSource;
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).FundingSource.Should().Be(PaymentEvent.FundingSourceType);
        }
        public static Array GetFundingSource()
        {
            return GetEnumValues<FundingSourceType>();
        }

        [Test]
        public void Maps_RequiredPaymentEventId()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).RequiredPaymentEventId.Should().Be(PaymentEvent.RequiredPaymentEventId);
        }

        [Test]
        public void Maps_Amount()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).Amount.Should().Be(PaymentEvent.AmountDue);
        }

        [Test]
        public void Maps_DeliveryPeriod()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).DeliveryPeriod.Should().Be(PaymentEvent.DeliveryPeriod);
        }

        [Test]
        public void Maps_PriceEpisodeIdentifier()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).PriceEpisodeIdentifier.Should().Be(PaymentEvent.PriceEpisodeIdentifier);
        }

        [Test]
        public void Maps_ApprenticeshipId()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).ApprenticeshipId.Should().Be(PaymentEvent.ApprenticeshipId);
        }

        [Test]
        public void Maps_ApprenticeshipPriceEpisodeId()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).ApprenticeshipPriceEpisodeId.Should().Be(PaymentEvent.ApprenticeshipPriceEpisodeId);
        }

        [Test]
        public void ShouldMapEarningsInfo()
        {
            PaymentEvent.StartDate = DateTime.UtcNow;
            PaymentEvent.PlannedEndDate = DateTime.UtcNow;
            PaymentEvent.ActualEndDate = DateTime.UtcNow;
            PaymentEvent.CompletionStatus = 2;
            PaymentEvent.CompletionAmount = 100M;
            PaymentEvent.InstalmentAmount = 200M;
            PaymentEvent.NumberOfInstalments = 5;

            var mapped = Mapper.Map<FundingSourceEventModel>(PaymentEvent);

            mapped.StartDate.Should().Be(PaymentEvent.StartDate);
            mapped.PlannedEndDate.Should().Be(PaymentEvent.PlannedEndDate);
            mapped.ActualEndDate.Should().Be(PaymentEvent.ActualEndDate);
            mapped.CompletionStatus.Should().Be(PaymentEvent.CompletionStatus);
            mapped.CompletionAmount.Should().Be(PaymentEvent.CompletionAmount);
            mapped.InstalmentAmount.Should().Be(PaymentEvent.InstalmentAmount);
            mapped.NumberOfInstalments.Should().Be(PaymentEvent.NumberOfInstalments);
        }

        protected static Array GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T));
        }

        public static Array GetContractTypes()
        {
            return GetEnumValues<ContractType>();
        }

        [Test]
        public void Maps_EventId()
        {
            var model = Mapper.Map<FundingSourceEventModel>(PaymentEvent);
            model.EventId.Should().Be(PaymentEvent.EventId);
        }

        [Test]
        public void Maps_CollectionPeriod()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).CollectionPeriod.Period.Should().Be(PaymentEvent.CollectionPeriod.Period);
        }

        [Test]
        public void Maps_CollectionYear()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).CollectionPeriod.AcademicYear.Should().Be(PaymentEvent.CollectionPeriod.AcademicYear);
        }

        [Test]
        public void Maps_EventTime()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).EventTime.Should().Be(PaymentEvent.EventTime);
        }

        [Test]
        public void Maps_IlrSubmissionDateTime()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).IlrSubmissionDateTime.Should().Be(PaymentEvent.IlrSubmissionDateTime);
        }

        [Test]
        public void Maps_JobId()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).JobId.Should().Be(PaymentEvent.JobId);
        }

        [Test]
        public void Maps_LearnerReferenceNumber()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearnerReferenceNumber.Should().Be(PaymentEvent.Learner.ReferenceNumber);
        }

        [Test]
        public void Maps_LearnerUln()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearnerUln.Should().Be(PaymentEvent.Learner.Uln);
        }

        [Test]
        public void Maps_LearningAimPathwayCode()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimPathwayCode.Should().Be(PaymentEvent.LearningAim.PathwayCode);
        }

        [Test]
        public void Maps_LearningAimFrameworkCode()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimFrameworkCode.Should().Be(PaymentEvent.LearningAim.FrameworkCode);
        }

        [Test]
        public void Maps_LearningAimFundingLineType()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimFundingLineType.Should().Be(PaymentEvent.LearningAim.FundingLineType);
        }

        [Test]
        public void Maps_LearningAimProgrammeType()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimProgrammeType.Should().Be(PaymentEvent.LearningAim.ProgrammeType);
        }

        [Test]
        public void Maps_LearningAimReference()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimReference.Should().Be(PaymentEvent.LearningAim.Reference);
        }

        [Test]
        public void Maps_LearningAimStandardCode()
        {
            Mapper.Map<FundingSourceEventModel>(PaymentEvent).LearningAimStandardCode.Should().Be(PaymentEvent.LearningAim.StandardCode);
        }


        protected virtual void PopulateCommonProperties(TSource paymentEvent)
        {
            paymentEvent.Learner = new Learner
            {
                ReferenceNumber = "LR-12345",
                Uln = 12345678
            };
            paymentEvent.CollectionPeriod = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
            paymentEvent.IlrSubmissionDateTime = DateTime.UtcNow;
            paymentEvent.JobId = 1234;
            paymentEvent.LearningAim = new LearningAim
            {
                FundingLineType = "funding line type",
                FrameworkCode = 99,
                StandardCode = 98,
                PathwayCode = 97,
                ProgrammeType = 96,
                Reference = "LA-54321"
            };
            paymentEvent.Ukprn = 23456;
            paymentEvent.AmountDue = 500;
            paymentEvent.DeliveryPeriod = 1;
            paymentEvent.PriceEpisodeIdentifier = "pe-1";
            paymentEvent.ApprenticeshipId = 300L;
            paymentEvent.ApprenticeshipPriceEpisodeId = 600L;
            paymentEvent.RequiredPaymentEventId = Guid.NewGuid();
            paymentEvent.SfaContributionPercentage = .9M;
        }
    }
}