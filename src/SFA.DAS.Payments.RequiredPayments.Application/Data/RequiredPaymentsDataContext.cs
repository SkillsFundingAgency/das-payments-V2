using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Data
{
    public class RequiredPaymentsDataContext : DbContext, IRequiredPaymentsDataContext
    {
        private readonly string connectionString;

        public RequiredPaymentsDataContext()
        {
        }

        public RequiredPaymentsDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public RequiredPaymentsDataContext(DbContextOptions options, string connectionString)
            : base(options)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public virtual DbSet<PaymentEntity> PaymentHistory { get; set; }
    }
}
