using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application
{
    public interface ISqlExceptionService
    {
        bool IsConstraintViolation(DbUpdateException dbUpdateException);
    }

    public class SqlExceptionService : ISqlExceptionService
    {
        public bool IsConstraintViolation(DbUpdateException dbUpdateException)
        {
            return dbUpdateException?.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 547 || sqlException.Number == 2601);
        }
    }
}