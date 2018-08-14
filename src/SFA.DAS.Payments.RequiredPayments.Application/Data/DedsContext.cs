using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Data
{
    public class DedsContext : DbContext
    {
        public virtual DbSet<PaymentEntity> PaymentHistory { get; set; }
    }
}
