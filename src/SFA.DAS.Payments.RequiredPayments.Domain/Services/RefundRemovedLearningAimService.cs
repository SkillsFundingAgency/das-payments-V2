using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RefundRemovedLearningAimService: IRefundRemovedLearningAimService
    {
        private readonly IRefundService refundService;
        private readonly IPaymentDueProcessor paymentDueProcessor;

        public RefundRemovedLearningAimService()
        {
            this.refundService = new RefundService();
            this.paymentDueProcessor = new PaymentDueProcessor();
        }

        public List<(byte deliveryPeriod, RequiredPayment payment)> RefundLearningAim(List<Payment> historicPayments)
        {
            return historicPayments
                .GroupBy(payment => payment.DeliveryPeriod)
                .SelectMany(group =>
                {
                    var historicPaymentsPerPeriod = group.ToList();
                    var payments = refundService.GetRefund(paymentDueProcessor.CalculateRequiredPaymentAmount(0, historicPaymentsPerPeriod), historicPaymentsPerPeriod);
                    return payments.Select(payment => (group.Key, payment));
                })
                .ToList();
        }
    }
}