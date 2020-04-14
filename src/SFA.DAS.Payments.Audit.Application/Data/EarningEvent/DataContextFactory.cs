using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
{
    public interface IDataContextFactory
    {
        IPaymentsDataContext Create(DbTransaction transaction = null);
    }

    public class DataContextFactory: IDataContextFactory
    {
        private readonly DbContextOptions<PaymentsDataContext> options;
        public DataContextFactory(IConfigurationHelper configHelper)
        {
            options = new DbContextOptionsBuilder<PaymentsDataContext>()
                .UseSqlServer(new SqlConnection(configHelper.GetConnectionString("PaymentsConnectionString")))
                .Options;
        }

        public IPaymentsDataContext Create(DbTransaction transaction = null)
        {
            var context = new PaymentsDataContext(options);
            if (transaction!=null)
                context.Database.UseTransaction(transaction);
            return context;
        }
    }
}