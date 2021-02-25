using System;
﻿using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Core.Data
{
    public static class Extensions
    {
        [Obsolete("Use SFA.DAS.Payments.Application.Data.Extensions instead")]
        public static bool IsUniqueKeyConstraintException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == 2601 || sqlException.Number == 2627;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        }

        [Obsolete("Use SFA.DAS.Payments.Application.Data.Extensions instead")]
        public static bool IsDeadLockException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == 1205;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && sqlEx.Number == 1205;
        }

        [Obsolete("Use SFA.DAS.Payments.Application.Data.Extensions instead")]
        public static bool IsTimeOutException(this Exception exception)
        {
            var sqlException = exception.GetException<SqlException>();
            if (sqlException != null)
                return sqlException.Number == -2;
            var sqlEx = exception.GetException<SqlException>();
            return sqlEx != null && sqlEx.Number == -2;
        }

        [Obsolete("Use SFA.DAS.Payments.Application.Data.Extensions instead")]
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


