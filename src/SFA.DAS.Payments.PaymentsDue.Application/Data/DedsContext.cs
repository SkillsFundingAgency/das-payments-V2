using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Application.Data
{
    public class DedsContext : DbContext
    {
        public virtual DbSet<PaymentEntity> PaymentHistory { get; set; }
    }
}
