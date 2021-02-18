using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public interface ICollectionPeriodService
    {
        void StoreCollectionPeriod(short academicYear, byte period, DateTime completionDateTime);
    }
}
