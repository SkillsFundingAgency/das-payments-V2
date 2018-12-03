using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Application.Mapping;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    public abstract class PeriodisedEventMappingTests<TSource, TDest> 
        where TSource : PeriodisedPaymentEvent
        where TDest : PeriodisedPaymentsEventModel
    {
        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.AddProfile<AuditProfile>();
                AddProfile(cfg);
            });
            Mapper.AssertConfigurationIsValid();
        }

        protected abstract void AddProfile(IMapperConfigurationExpression cfg);
        protected TSource PaymentEvent { get; set; }
        protected abstract TSource CreatePaymentEvent();

        [SetUp]
        public void SetUp()
        {
            PaymentEvent = CreatePaymentEvent();
            PopulateCommonProperties(PaymentEvent);
        }

        protected virtual void PopulateCommonProperties(TSource paymentEvent)
        {
            paymentEvent.Learner = new Learner
            {
                ReferenceNumber = "LR-12345",
                Uln = 12345678
            };
            paymentEvent.AmountDue = 500;
            paymentEvent.CollectionPeriod = new CalendarPeriod(2018,12);
            paymentEvent.DeliveryPeriod = new CalendarPeriod(2018,1);
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
            paymentEvent.PriceEpisodeIdentifier = "pe-1";
            paymentEvent.Ukprn = 23456;
        }

        [Test]
        public void Maps_EventId()
        {
            var model = Mapper.Map<FundingSourceEventModel>(PaymentEvent);
            model.EventId.Should().Be(PaymentEvent.EventId);
            //model.AgreementId, opt => opt.Ignore())
            //model.TransactionType, opt => opt.Ignore())
            //model.SfaContributionPercentage, opt => opt.Ignore())
            //Mapper.Map<FundingSourceEventModel>(PaymentEvent).ContractType, opt => opt.Ignore();
        }

        [Test]
        public void Maps_CollectionPeriod()
        {
            Mapper.Map<TDest>(PaymentEvent).CollectionPeriod.Should().Be(PaymentEvent.CollectionPeriod.Period);
        }

        [Test]
        public void Maps_CollectionYear()
        {
            Mapper.Map<TDest>(PaymentEvent).CollectionYear.Should().Be(PaymentEvent.CollectionPeriod.GetCollectionYear());
        }

        [Test]
        public void Maps_Amount()
        {
            Mapper.Map<TDest>(PaymentEvent).Amount.Should().Be(PaymentEvent.AmountDue);
        }

        [Test]
        public void Maps_DeliveryPeriod()
        {
            Mapper.Map<TDest>(PaymentEvent).DeliveryPeriod.Should().Be(PaymentEvent.DeliveryPeriod.Period);
        }

        [Test]
        public void Maps_EventTime()
        {
            Mapper.Map<TDest>(PaymentEvent).EventTime.Should().Be(PaymentEvent.EventTime);
        }

        [Test]
        public void Maps_IlrSubmissionDateTime()
        {
            Mapper.Map<TDest>(PaymentEvent).IlrSubmissionDateTime.Should().Be(PaymentEvent.IlrSubmissionDateTime);
        }

        [Test]
        public void Maps_JobId()
        {
            Mapper.Map<TDest>(PaymentEvent).JobId.Should().Be(PaymentEvent.JobId);
        }

        [Test]
        public void Maps_LearnerReferenceNumber()
        {
            Mapper.Map<TDest>(PaymentEvent).LearnerReferenceNumber.Should().Be(PaymentEvent.Learner.ReferenceNumber);
        }

        [Test]
        public void Maps_LearnerUln()
        {
            Mapper.Map<TDest>(PaymentEvent).LearnerUln.Should().Be(PaymentEvent.Learner.Uln);
        }

        [Test]
        public void Maps_LearningAimPathwayCode()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimPathwayCode.Should().Be(PaymentEvent.LearningAim.PathwayCode);
        }

        [Test]
        public void Maps_LearningAimFrameworkCode()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimFrameworkCode.Should().Be(PaymentEvent.LearningAim.FrameworkCode);
        }

        [Test]
        public void Maps_LearningAimFundingLineType()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimFundingLineType.Should().Be(PaymentEvent.LearningAim.FundingLineType);
        }

        [Test]
        public void Maps_LearningAimProgrammeType()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimProgrammeType.Should().Be(PaymentEvent.LearningAim.ProgrammeType);
        }

        [Test]
        public void Maps_LearningAimReference()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimReference.Should().Be(PaymentEvent.LearningAim.Reference);
        }

        [Test]
        public void Maps_LearningAimStandardCode()
        {
            Mapper.Map<TDest>(PaymentEvent).LearningAimStandardCode.Should().Be(PaymentEvent.LearningAim.StandardCode);
        }

    }
}