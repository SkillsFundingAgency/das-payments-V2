using System.Configuration;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Data
{
    public class RequiredPaymentsDataContext : DbContext, IRequiredPaymentsDataContext
    {
        private string _connectionString;

        public RequiredPaymentsDataContext(string connectionString)
            : base()
        {
            _connectionString = connectionString;
        }
        public RequiredPaymentsDataContext(DbContextOptions<RequiredPaymentsDataContext> options, string connectionString)
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
