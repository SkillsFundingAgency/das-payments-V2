using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace PaymentTools.Pages
{
    public class ConfigurablePaymentsDataContext : PaymentsDataContext
    {
        public ConfigurablePaymentsDataContext(DbContextOptions<ConfigurablePaymentsDataContext> options) 
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // do not call base
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder, enforceRequired: false);
        }
    }
}