using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public interface ICalculatePeriodStartAndEndDate
    {
        (DateTime periodStartDate, DateTime periodEndDate) GetPeriodDate(int period, int academicYear);
    }
}
