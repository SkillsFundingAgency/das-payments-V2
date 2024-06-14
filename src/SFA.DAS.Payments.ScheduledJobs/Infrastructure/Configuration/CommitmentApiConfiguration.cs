namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.Configuration
{
    public interface ICommitmentApiConfiguration
    {
        string ApiBaseUrl { get; set; }

        string ClientId { get; set; }

        string ClientSecret { get; set; }

        string IdentifierUri { get; set; }

        string Tenant { get; set; }

    }

    public class CommitmentApiConfiguration : ICommitmentApiConfiguration
    {
        public string ApiBaseUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string IdentifierUri { get; set; }

        public string Tenant { get; set; }
    }
}