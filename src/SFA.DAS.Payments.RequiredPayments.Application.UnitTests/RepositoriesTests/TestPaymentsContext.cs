using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.RepositoriesTests
{
    public class TestPaymentsContext : PaymentsDataContext
    {
        public TestPaymentsContext(DbContextOptions<PaymentsDataContext> options) : base(options)
        {}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }
    }
}