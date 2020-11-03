using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd
{
    public class MetricsValidationService : IMetricsValidationService
    {
        private readonly string _authCode;
        private readonly string _functionAddress;

        public MetricsValidationService()
        {
            _authCode = string.Empty; //todo set this from config and auto to string.empty if dev (can be in the config too so nothing clever here)
            _functionAddress = "http://localhost:7071/api/ValidateSubmissionWindow"; //todo set this from config
        }

        public async Task<bool> Validate(long jobId, short academicYear, byte collectionPeriod)
        {
            var result = await new HttpClient().GetAsync(BuildUriFromParameters(jobId, academicYear, collectionPeriod));
            return result.StatusCode == HttpStatusCode.OK;
        }

        private string BuildUriFromParameters(long jobId, short academicYear, byte collectionPeriod)
        {
            return string.IsNullOrWhiteSpace(_authCode)
                ? $"{_functionAddress}?jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}"
                : $"{_functionAddress}?code={_authCode}&jobId={jobId}&collectionPeriod={collectionPeriod}&AcademicYear={academicYear}";
            //return string.IsNullOrWhiteSpace(_authCode)
            //    ? $"{_functionAddress}?jobId={{{jobId}}}&collectionPeriod={{{collectionPeriod}}}&AcademicYear={{{academicYear}}}"
            //    : $"{_functionAddress}?code={{{_authCode}}}&jobId={{{jobId}}}&collectionPeriod={{{collectionPeriod}}}&AcademicYear={{{academicYear}}}";
        }
    }

    //public interface IMetricsValidationRequestBuilder
    //{
    //    string BuildFromParameters(long jobId, short academicYear, byte collectionPeriod);
    //}

    //public class MetricsValidationRequestBuilder : IMetricsValidationRequestBuilder
    //{
    //    private readonly string _authCode;
    //    private readonly string _functionAddress;

    //    public MetricsValidationRequestBuilder()
    //    {
    //        _authCode = string.Empty; //todo set this from config and auto to string.empty if dev
    //        _functionAddress = "http://localhost:7071/api/ValidateSubmissionWindow";
    //    }

    //    public string BuildFromParameters(long jobId, short academicYear, byte collectionPeriod)
    //    {
    //        return string.IsNullOrWhiteSpace(_authCode)
    //            ? $"{_functionAddress}?jobId={{{jobId}}}&collectionPeriod={{{collectionPeriod}}}&AcademicYear={{{academicYear}}}"
    //            : $"{_functionAddress}?code={{{_authCode}}}&jobId={{{jobId}}}&collectionPeriod={{{collectionPeriod}}}&AcademicYear={{{academicYear}}}";
    //    }
    //}
}