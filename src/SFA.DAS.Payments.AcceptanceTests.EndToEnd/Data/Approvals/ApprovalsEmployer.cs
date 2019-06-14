namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class ApprovalsEmployer
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string AgreementId { get; set; }
        public long AccountId { get; set; }
        public ApprovalsEmployer()
        {
            Identifier = "Employer A";
        }
    }
}