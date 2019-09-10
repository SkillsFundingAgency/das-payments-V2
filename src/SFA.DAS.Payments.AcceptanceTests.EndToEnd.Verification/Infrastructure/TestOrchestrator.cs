using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Jobs.Model;
using Polly;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Verification.Infrastructure;
using SFA.DAS.Payments.AcceptanceTests.Services;
using SFA.DAS.Payments.AcceptanceTests.Services.Exceptions;
using SFA.DAS.Payments.AcceptanceTests.Services.Intefaces;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.ComparisonTesting
{
    public interface ITestOrchestrator {
        Task<IEnumerable<string>> SetupTestFiles();

        Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist);

        Task DeleteTestFiles(IEnumerable<string> filelist);
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

        public Task<IEnumerable<FileUploadJob>> SubmitFiles(IEnumerable<string> filelist)
        {
            return submissionService.SubmitFiles(filelist);
        }

        public Task DeleteTestFiles(IEnumerable<string> filelist)
        {
            return submissionService.DeleteFiles(filelist);
        }
    }
}
