using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping
{
    public abstract class PeriodisedEventMappingTests<TSource, TDest> : PaymentEventMappingTests<TSource,TDest>
        where TSource : PeriodisedPaymentEvent
        where TDest : PeriodisedPaymentsEventModel
    {

        protected override void PopulateCommonProperties(TSource paymentEvent)
        {
            base.PopulateCommonProperties(paymentEvent);
            paymentEvent.AmountDue = 500;
            paymentEvent.DeliveryPeriod = 1;
            paymentEvent.PriceEpisodeIdentifier = "pe-1";
            paymentEvent.ApprenticeshipId = 300L;
            paymentEvent.ApprenticeshipPriceEpisodeId = 600L;
        }


        [Test]
        public void Maps_Amount()
        {
            Mapper.Map<TDest>(PaymentEvent).Amount.Should().Be(PaymentEvent.AmountDue);
        }

        [Test]
        public void Maps_DeliveryPeriod()
        {
            Mapper.Map<TDest>(PaymentEvent).DeliveryPeriod.Should().Be(PaymentEvent.DeliveryPeriod);
        }

        [Test]
        public void Maps_PriceEpisodeIdentifier()
        {
            Mapper.Map<TDest>(PaymentEvent).PriceEpisodeIdentifier.Should().Be(PaymentEvent.PriceEpisodeIdentifier);
        }

        [Test]
        public void Maps_ApprenticeshipId()
        {
            Mapper.Map<TDest>(PaymentEvent).ApprenticeshipId.Should().Be(PaymentEvent.ApprenticeshipId);
        }

        [Test]
        public void Maps_ApprenticeshipPriceEpisodeId()
        {
            Mapper.Map<TDest>(PaymentEvent).ApprenticeshipPriceEpisodeId.Should().Be(PaymentEvent.ApprenticeshipPriceEpisodeId);
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

            var mapped = Mapper.Map<TDest>(PaymentEvent);

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