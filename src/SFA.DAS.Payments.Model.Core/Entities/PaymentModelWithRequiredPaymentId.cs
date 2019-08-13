using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public class PaymentModelWithRequiredPaymentId : PaymentModel
    {
        public Guid RequiredPaymentId { get; set; }
        public int? LearningAimSequenceNumber { get; set; }
        public decimal? AmountDue { get; set; }
    }
}
