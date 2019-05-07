using System;
using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RefundRemovedLearningAimService: IRefundRemovedLearningAimService
    {
        private readonly IRefundService refundService;

        public RefundRemovedLearningAimService(IRefundService refundService)
        {
            this.refundService = refundService ?? throw new ArgumentNullException(nameof(refundService));
        }

        public List<RequiredPayment> RefundLearningAim(List<Payment> historicPayments)
        {
            throw new NotImplementedException();
        }
    }
}