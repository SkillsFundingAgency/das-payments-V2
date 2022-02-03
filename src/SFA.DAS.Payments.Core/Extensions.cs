using System;
﻿using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.Core
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static Dictionary<string, string> ConcatDictionary(this Dictionary<string, string> first,
            Dictionary<string, string> second)
        {
            return second == null ? first : first.Concat(second).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static decimal AsRounded(this decimal unrounded)
        {
            return Math.Round(unrounded, 5);
        }

        public static decimal? AsRounded(this decimal? unrounded)
        {
            return unrounded.HasValue ? AsRounded((decimal?) unrounded.Value) : default(decimal?);
        }
        
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
		
        public static byte GetPeriodFromDate(this DateTime date)
        {
            byte period;
            var month = date.Month;

            if (month < 8)
            {
                period = (byte) (month + 5);
            }
            else
            {
                period = (byte) (month - 7);
            }
            return period;
        }
    }
}


