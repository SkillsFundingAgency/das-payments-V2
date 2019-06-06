using System;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}