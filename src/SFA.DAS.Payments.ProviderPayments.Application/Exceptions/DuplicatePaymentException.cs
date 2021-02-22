using System;
using Microsoft.Data.SqlClient;

namespace SFA.DAS.Payments.ProviderPayments.Application.Exceptions
{
    public class DuplicatePaymentException : Exception
    {
        public DuplicatePaymentException() : base("Duplicate payment exception") { }
        public DuplicatePaymentException(Exception innerException) : base("Duplicate payment exception.", innerException) { }
        public DuplicatePaymentException(string message, Exception innerException) : base(message, innerException) { }
    }
}