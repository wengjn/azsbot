using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zummer.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Request status. Should map precisely with AscSdk.AggregatorPlugin.Services.DiagnosticService.DiagnosticServiceProcessingStatus.
    /// </summary>
    public enum ProcessingStatus
    {
        /// <summary>
        /// Request is queued for processing
        /// </summary>
        Queued = 0,

        /// <summary>
        /// Request is being processed
        /// </summary>
        Running = 1,

        /// <summary>
        /// Processing succeeded
        /// </summary>
        Succeeded = 2,

        /// <summary>
        /// Processing failed
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Processing aborted
        /// </summary>
        Aborted = 4,

        /// <summary>
        /// Prerequisites missing
        /// </summary>
        MissingPrerequisite = 5,

        /// <summary>
        /// Processing timed out
        /// </summary>
        Timeout = 6,

        /// <summary>
        /// Invalid request
        /// </summary>
        InvalidRequest = 7
    }

    public class DiagnosticResponseBase
    {
        /// <summary>
        /// Session Id from diagnostic request
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Request Id
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets request submit time for diagnostic request
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// Gets or sets the message, generally when the diagnostic was entirely unable to execute
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the error encountered during the diagnostic
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Processing status of request
        /// </summary>
        public ProcessingStatus Status { get; set; }

        /// <summary>
        /// Gets or sets symptomId
        /// </summary>
        public string SymptomId { get; set; }

        /// <summary>
        /// Gets or sets the parameters originally sent when this diagnostic was executed
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the list of tags attached to this response
        /// </summary>
        public List<string> Tags { get; set; }
    }

    /// <summary>
    /// Diagnostic pre-fetch data
    /// </summary>
    public class DiagnosticPreFetchData
    {
        /// <summary>
        /// Prefetch data for Resource Explorer
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> ResourceExplorerData { get; set; }

        /// <summary>
        /// Prefetch data for Subscription Rdfe.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionRdfeData { get; set; }

        /// <summary>
        /// Prefetch daa for Subscription Crp
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionCrpData { get; set; }

        /// <summary>
        /// Prefetch daa for Subscription Crp summary
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionCrpSummaryData { get; set; }

        /// <summary>
        /// Gets or sets the subscription rdfe operations data.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionRdfeOperationsData { get; set; }

        /// <summary>
        /// Gets or sets the subscription CRP operations data.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionCrpOperationsData { get; set; }

        /// <summary>
        /// Gets or sets the virtual machine view.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> VirtualMachineView { get; set; }

        /// <summary>
        /// Gets or sets Subscription Nrp Data
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionNrpData { get; set; }

        /// <summary>
        /// Gets or sets Networking NRP View
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> SubscriptionRdfeNetworkData { get; set; }

        /// <summary>
        /// Gets or sets vm screenshot data
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> VmScreenshotData { get; set; }

        /// <summary>
        /// Gets or sets vm console log data
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> VmConsoleLogData { get; set; }

        /// <summary>
        /// Gets or sets vm inspectIaaSDisk data
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> VMInspectIaaSDisk { get; set; }

        /// <summary>
        /// Gets or sets the Storage view.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> StorageData { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous worker and gateway manager logs VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> AsyncWorkerAndGatewayManagerLogsVpnRca { get; set; }

        /// <summary>
        /// Gets or sets the BLOB lease operation logs VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> BlobLeaseOperationLogsVpnRca { get; set; }

        /// <summary>
        /// Gets or sets the role instance perf stat VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> RoleInstancePerfStatVpnRca { get; set; }

        /// <summary>
        /// Gets or sets the tenant connection stats VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> TenantConnectionStatsVpnRca { get; set; }

        /// <summary>
        /// Gets or sets the tenant events VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> TenantEventsVpnRca { get; set; }

        /// <summary>
        /// Gets the tenant history VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> TenantHistoryVpnRca { get; set; }

        /// <summary>
        /// Gets the tenant logs VPN rca.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> TenantLogsVpnRca { get; set; }

        /// <summary>
        /// Gets or sets the ike parser.
        /// </summary>
        public IReadOnlyList<DiagnosticResponse> IkeParser { get; set; }
    }

    /// <summary>
    /// Response from diagnosis endpoint
    /// </summary>
    public class DiagnosticResponse : DiagnosticResponseBase
    {
        public const string BlobUrl = "BlobUrl";

        public const string ArmBlobUrl = "armBlobUrl";

        public const string RdfeBlobUrl = "rdfeBlobUrl";

        public const string CrpBlobUrl = "crpBlobUrl";

        public const string Result = "Result";

        /// <summary>
        /// Gets or sets diagnostic data
        /// </summary>
        public Dictionary<string, string> DiagnosticData { get; set; }

        /// <summary>
        /// List of Problems
        /// </summary>
        public Problem[] Problems { get; set; }
    }

    public class Problem
    {
        /// <summary>
        /// Gets or sets Localized message title that will be displayed to user by client
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Localized message that will be displayed to user by client
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets list of repairs
        /// </summary>
        public Repair[] Repairs { get; set; }

        /// <summary>
        /// Gets or sets additional details related to problem found
        /// </summary>
        public Dictionary<string, string> AdditionalDetails { get; set; }
    }

    public class Repair
    {
        /// <summary>
        /// Gets or sets repair title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets repair message
        /// </summary>
        public string Message { get; set; }
    }
}