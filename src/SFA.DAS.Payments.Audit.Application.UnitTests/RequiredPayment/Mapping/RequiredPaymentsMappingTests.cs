using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.RequiredPayment.Mapping
{
    public abstract class RequiredPaymentsMappingTests<TSource>
        where TSource : PeriodisedRequiredPaymentEvent
    {
        protected virtual void AddProfile(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<RequiredPaymentProfile>();
        }

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


        protected static Array GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T));
        }

        public static Array GetContractTypes()
        {
            return GetEnumValues<ContractType>();
        }

        [Test]
        public void Maps_EarningEventId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).EarningEventId.Should().Be(PaymentEvent.EarningEventId);
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
            paymentEvent.EarningEventId = Guid.NewGuid();
            paymentEvent.AmountDue = 500;
            paymentEvent.DeliveryPeriod = 1;
            paymentEvent.PriceEpisodeIdentifier = "pe-1";
            paymentEvent.ApprenticeshipId = 300L;
            paymentEvent.ApprenticeshipPriceEpisodeId = 600L;
        }
        [Test]
        public void Maps_Amount()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).Amount.Should().Be(PaymentEvent.AmountDue);
        }

        [Test]
        public void Maps_DeliveryPeriod()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).DeliveryPeriod.Should().Be(PaymentEvent.DeliveryPeriod);
        }

        [Test]
        public void Maps_PriceEpisodeIdentifier()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).PriceEpisodeIdentifier.Should().Be(PaymentEvent.PriceEpisodeIdentifier);
        }

        [Test]
        public void Maps_ApprenticeshipId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).ApprenticeshipId.Should().Be(PaymentEvent.ApprenticeshipId);
        }

        [Test]
        public void Maps_ApprenticeshipPriceEpisodeId()
        {
            Mapper.Map<RequiredPaymentEventModel>(PaymentEvent).ApprenticeshipPriceEpisodeId.Should().Be(PaymentEvent.ApprenticeshipPriceEpisodeId);
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

            var mapped = Mapper.Map<RequiredPaymentEventModel>(PaymentEvent);

            mapped.StartDate.Should().Be(PaymentEvent.StartDate);
            mapped.PlannedEndDate.Should().Be(PaymentEvent.PlannedEndDate);
            mapped.ActualEndDate.Should().Be(PaymentEvent.ActualEndDate);
            mapped.CompletionStatus.Should().Be(PaymentEvent.CompletionStatus);
            mapped.CompletionAmount.Should().Be(PaymentEvent.CompletionAmount);
            mapped.InstalmentAmount.Should().Be(PaymentEvent.InstalmentAmount);
            mapped.NumberOfInstalments.Should().Be(PaymentEvent.NumberOfInstalments);
        }

    }


}