using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
  public   class FakePaymentsLogger :IPaymentLogger
    {
        public void Dispose()
        {
           

        }

        public void LogFatal(string message, Exception exception = null, object[] parameters = null, long jobIdOverride = -1,
            string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
         

        }

        public void LogError(string message, Exception exception = null, object[] parameters = null, long jobIdOverride = -1,
            string callerMemberName = "", string callerFilePath = "", int callerLineNumber = 0)
        {
            

        }

        public void LogWarning(string message, object[] parameters = null, long jobIdOverride = -1, string callerMemberName = "",
            string callerFilePath = "", int callerLineNumber = 0)
        {


        }

        public void LogDebug(string message, object[] parameters = null, long jobIdOverride = -1, string callerMemberName = "",
            string callerFilePath = "", int callerLineNumber = 0)
        {
         

        }

        public void LogInfo(string message, object[] parameters = null, long jobIdOverride = -1, string callerMemberName = "",
            string callerFilePath = "", int callerLineNumber = 0)
        {
           

        }

        public void LogVerbose(string message, object[] parameters = null, long jobIdOverride = -1, string callerMemberName = "",
            string callerFilePath = "", int callerLineNumber = 0)
        {
           

        }
    }
}
