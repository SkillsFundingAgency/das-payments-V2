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

    }
}