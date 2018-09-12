using System;

namespace SFA.DAS.Payments.FundingSource.Domain.Exceptions
{
    public class FundingSourceRequiredPaymentValidationException : Exception
    {
        public FundingSourceRequiredPaymentValidationException(string msg):base(msg)
        {

        }
    }
}