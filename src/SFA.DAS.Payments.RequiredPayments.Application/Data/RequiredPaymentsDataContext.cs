using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Data
{
    public class RequiredPaymentsDataContext : DbContext, IRequiredPaymentsDataContext
    {
        private readonly string _connectionString;

        public RequiredPaymentsDataContext()
        {
        }

        public RequiredPaymentsDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RequiredPaymentsDataContext(DbContextOptions options, string connectionString)
            : base(options)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public virtual DbSet<PaymentEntity> PaymentHistory { get; set; }
    }
}
