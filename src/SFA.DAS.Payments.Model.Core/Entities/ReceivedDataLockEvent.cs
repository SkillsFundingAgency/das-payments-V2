namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class ReceivedDataLockEvent
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
    }
}