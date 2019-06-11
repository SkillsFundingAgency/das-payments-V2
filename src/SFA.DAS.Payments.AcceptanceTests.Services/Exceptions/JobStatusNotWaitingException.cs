namespace SFA.DAS.Payments.AcceptanceTests.Services.Exceptions
{
    using System;

    public class JobStatusNotWaitingException : Exception
    {
        public JobStatusNotWaitingException()
        {
        }

        public JobStatusNotWaitingException(string message)
            : base(message)
        {
        }

        public JobStatusNotWaitingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
