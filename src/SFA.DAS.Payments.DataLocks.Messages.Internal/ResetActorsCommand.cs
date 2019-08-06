using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Messages.Internal
{
    public class ResetActorsCommand
    {
        public List<long> Ulns { get; set; }
    }
}