using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    public abstract class PaymentEventMappingTests<TSource, TDest>
        where TSource :PaymentsEvent
        where TDest : PaymentsEventModel
    {
        protected IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            var config = new MapperConfiguration(AddProfile);
            Mapper = new Mapper(config);
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
            var model = Mapper.Map<TDest>(PaymentEvent);
            model.EventId.Should().Be(PaymentEvent.EventId);
        }

        [Test]
        public void Maps_CollectionPeriod()
        {
            Mapper.Map<TDest>(PaymentEvent).CollectionPeriod.Should().Be(PaymentEvent.CollectionPeriod.Period);
        }

        [Test]
        public void Maps_CollectionYear()
        {
            Mapper.Map<TDest>(PaymentEvent).AcademicYear.Should().Be(PaymentEvent.CollectionPeriod.AcademicYear);
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