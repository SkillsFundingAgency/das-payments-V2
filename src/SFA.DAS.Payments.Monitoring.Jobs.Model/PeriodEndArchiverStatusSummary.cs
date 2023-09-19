namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class PeriodEndArchiverStatusSummary
    {
        public bool EntityExists { get; set; }
        public ArchiveEntityState EntityState { get; set; }
    }
}