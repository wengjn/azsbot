using System.Collections.Generic;
using System.Threading.Tasks;
using Zummer.Models.Search;

namespace Zummer.Services
{
    public interface IAzureDiagnosticService
    {
        Task<string> GetDiagnosticResultWith(string requestId);

        Task<string> StartDiagnosticRequest(string symptom, Dictionary<string, string> parameters);

        Task<DiagnosticResponse> GetDataByPollModeAsync(string symptom, Dictionary<string, string> parameters, int maxTimeInMs = 200000, int retryTime = 500, bool usingCachedData = true, int maxCachedDuration = 600);
    }
}
