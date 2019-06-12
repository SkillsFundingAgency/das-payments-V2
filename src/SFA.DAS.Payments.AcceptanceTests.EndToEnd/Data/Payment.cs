using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Payment
    {
        public string CollectionPeriod { get; set; }
        private CollectionPeriod parsedCollectionPeriod;

        public CollectionPeriod ParsedCollectionPeriod => parsedCollectionPeriod ??
                                                          (parsedCollectionPeriod = new CollectionPeriodBuilder()
                                                              .WithSpecDate(this.CollectionPeriod).Build());
        public string DeliveryPeriod { get; set; }
        private CollectionPeriod parsedDeliveryPeriod;
        public CollectionPeriod ParsedDeliveryPeriod => parsedDeliveryPeriod ??
                                                        (parsedDeliveryPeriod = new CollectionPeriodBuilder()
                                                            .WithSpecDate(DeliveryPeriod).Build());
        public decimal OnProgramme { get; set; }
        public decimal Completion { get; set; }
        public decimal Balancing { get; set; }
        public IDictionary<IncentivePaymentType, decimal> IncentiveValues { get; set; } = new Dictionary<IncentivePaymentType, decimal>();
    }
}
