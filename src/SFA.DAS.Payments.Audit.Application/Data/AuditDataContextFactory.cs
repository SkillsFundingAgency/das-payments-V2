using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Audit.Application.Data
{
    public interface IAuditDataContextFactory
    {
        IAuditDataContext Create(DbTransaction transaction = null);
    }

    public class AuditDataContextFactory: IAuditDataContextFactory
    {
        private readonly DbContextOptions<AuditDataContext> options;
        public AuditDataContextFactory(IConfigurationHelper configHelper)
        {
            options = new DbContextOptionsBuilder<AuditDataContext>()
                .UseSqlServer(new SqlConnection(configHelper.GetConnectionString("PaymentsConnectionString")))
                .Options;
        }

        public IAuditDataContext Create(DbTransaction transaction = null)
        {
            var context = new AuditDataContext(options);
            if (transaction!=null)
                context.Database.UseTransaction(transaction);
            return context;
        }
    }
}