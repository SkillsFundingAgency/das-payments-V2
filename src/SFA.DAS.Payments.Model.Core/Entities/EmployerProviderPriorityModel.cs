using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class EmployerProviderPriorityModel
    {
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public long Ukprn { get; set; }
        public int Order { get; set; }
    }
}
