using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application
{
    public interface IRequiredPaymentsDataContext
    {
        DbSet<PaymentEntity> PaymentHistory { get; }
    }
}