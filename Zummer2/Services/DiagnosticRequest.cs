using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zummer.Services
{
    using System.Collections.Generic;

    public class DiagnosticRequest
    {
        /// <summary>
        /// Scenario name that you want to run diagnostic on, eg. CannotRDP, EGData
        /// </summary>
        public string SymptomId { get; set; }

        /// <summary>
        /// required parameters by each scenario as a dictionary
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Timeout value set, optional
        /// </summary>
        public long? TimeoutInMs { get; set; }

        /// <summary>
        /// Gets or sets the tags to associate with this request so it can be queried later
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow Ads cached result].
        /// </summary>
        public bool AllowExistingResult { get; set; } = true;

        /// <summary>
        /// Gets or sets the max lifetime of existing result to retrieve
        /// </summary>
        public long? ResultLifetimeInSeconds { get; set; } = 600;
    }
}