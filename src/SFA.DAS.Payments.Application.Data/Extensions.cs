using System;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Payments.Application.Data
{
    public static class Extensions
    {
        public static bool IsUniqueKeyConstraintException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == 2601 || sqlException.Number == 2627;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        public static bool IsDeadLockException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == 1205;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && sqlEx.Number == 1205;
        }

        public static bool IsTimeOutException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == -2;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && sqlEx.Number == -2;
        }

        private static T GetException<T>(this Exception e) where T : Exception
        {
            var innerEx = e;
            while (innerEx != null && !(innerEx is T))
            {
                innerEx = innerEx.InnerException;
            }

            return innerEx as T;
        }
    }
}
