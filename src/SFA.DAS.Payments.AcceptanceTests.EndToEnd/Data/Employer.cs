namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Employer
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string AgreementId { get; set; }

        public Employer()
        {
            Identifier = "Employer A";
        }
    }
}