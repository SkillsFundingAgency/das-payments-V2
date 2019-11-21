namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Entities
{
    public class PaymentsValues
    {
        public decimal? RequiredPaymentsThisMonth { get; set; }
        public decimal? PaymentsPriorToThisMonthYtd { get; set; }
        public decimal? ExpectedPaymentsAfterPeriodEnd { get; set; }
        public decimal? TotalPaymentsThisMonth { get; set; }
        public decimal? TotalAct1Ytd { get; set; }
        public decimal? TotalAct2Ytd { get; set; }
        public decimal? TotalPaymentsYtd { get; set; }
        public decimal? HeldBackCompletionThisMonth { get; set; }
        public decimal? DasEarnings { get; set; }
        public decimal? DataLockedEarnings { get; set; }
        public decimal? DataLockedPayments { get; set; }
        public decimal? AdjustedDataLocks { get; set; }
    }
}