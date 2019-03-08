using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using Earning = SFA.DAS.Payments.Model.Core.OnProgramme.Earning;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RequiredPaymentService
    {
        private IPaymentDueProcessor paymentsDue;
        private IRefundService refunds;

        public RequiredPaymentService(IPaymentDueProcessor paymentsDue, IRefundService refunds)
        {
            this.paymentsDue = paymentsDue;
            this.refunds = refunds;
        }

        public List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory)
        {
            throw new System.NotImplementedException();
        }
    }
}
