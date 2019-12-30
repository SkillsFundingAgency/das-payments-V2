using System;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public class RawEarningsDataContext: DbContext
    {
        private readonly string connectionString;

        public RawEarningsDataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}