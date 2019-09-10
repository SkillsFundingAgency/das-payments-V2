using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure
{
    public interface ITestOrchestrator
    {
        Task<IEnumerable<string>> SetupTestFiles();

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList);

        Task DeleteTestFiles(IEnumerable<string> fileList);
    }

    public class TestOrchestrator : ITestOrchestrator
    {
        private readonly ISubmissionService submissionService;

        public TestOrchestrator(ISubmissionService submissionService)
        {
            this.submissionService = submissionService;
        }

        public async Task<IEnumerable<string>> SetupTestFiles()
        {
            var playlist = await submissionService.ImportPlaylist();
            await submissionService.ClearPaymentsData(playlist);
            return await submissionService.CreateTestFiles(playlist);
        }

        public Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> fileList)
        {
            return submissionService.SubmitFiles(fileList);
        }

        public Task DeleteTestFiles(IEnumerable<string> fileList)
        {
            return submissionService.DeleteFiles(fileList);
        }
    }
}