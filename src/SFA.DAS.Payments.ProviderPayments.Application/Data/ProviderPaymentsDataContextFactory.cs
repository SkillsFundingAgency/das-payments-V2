using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Application.Data
{
    public interface IProviderPaymentsDataContextFactory
    {
        IProviderPaymentsDataContext Create(DbTransaction transaction = null);
    }

    public class ProviderPaymentsDataContextFactory : IProviderPaymentsDataContextFactory
    {
        private readonly DbContextOptions<ProviderPaymentsDataContext> options;
        public ProviderPaymentsDataContextFactory(IConfigurationHelper configHelper)
        {
            options = new DbContextOptionsBuilder<ProviderPaymentsDataContext>()
                .UseSqlServer(new SqlConnection(configHelper.GetConnectionString("PaymentsConnectionString")))
                .Options;
        }

        public IProviderPaymentsDataContext Create(DbTransaction transaction = null)
        {
            var context = new ProviderPaymentsDataContext(options);
            if (transaction != null)
                context.Database.UseTransaction(transaction);
            return context;
        }
    }
}