using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zummer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class DiagnosticServiceUrlBuilder
    {
        private const string DataServicePath = "api/diagnosticdata";

        private const string PrefetchServicePath = "api/sr";

        private const string BatchServicePath = "api/diagnosticdata/batch";

        private readonly string servicePath;

        private readonly List<KeyValuePair<string, string>> urlParams;

        public DiagnosticServiceUrlBuilder(Service service)
        {
            switch (service)
            {
                case Service.DiagnosticData:
                    this.servicePath = DataServicePath;
                    break;
                case Service.DiagnosticBatch:
                    this.servicePath = BatchServicePath;
                    break;
                case Service.Prefetch:
                    this.servicePath = PrefetchServicePath;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unsupported service value: " + service);
            }

            this.urlParams = new List<KeyValuePair<string, string>>();
        }

        public enum Service
        {
            DiagnosticData,
            DiagnosticBatch,
            Prefetch
        }

        /// <summary>
        /// Adds the given values with the given key.
        /// ADS uses the following scheme for keys with multiple values (in most cases): &amp;abc=1&amp;abc=2&amp;abc=3
        /// </summary>
        /// <param name="key">The URL parameter key</param>
        /// <param name="values">The values to provide under this key</param>
        public void AddParam(string key, params string[] values)
        {
            if (values != null)
            {
                this.urlParams.AddRange(values.Select(item => new KeyValuePair<string, string>(key, item)));
            }
        }

        /// <summary>
        /// Adds the given datetime as a URL parameter.
        /// The value is formatted to match the format specified in ADS documentation for the "search" endpoint.
        /// The date will be converted to UTC: be sure to specify a time zone when constructing a new DateTime object.
        /// </summary>
        /// <param name="key">The URL parameter key</param>
        /// <param name="value">The date to provide as a value. Will be converted to M/d/yyyy HH:mm:ss format (URL-encoded)</param>
        public void AddParam(string key, DateTime? value = null)
        {
            if (value != null)
            {
                this.AddParam(key, value.Value.ToUniversalTime().ToString("M/d/yyyy HH:mm:ss"));
            }
        }

        public string GetUrlParamString()
        {
            return string.Join("&", this.urlParams.Where(a => a.Value != null).Select(a => a.Key + "=" + Uri.EscapeDataString(a.Value)));
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(this.servicePath);

            var urlParamString = this.GetUrlParamString();
            if (urlParamString.Length > 0)
            {
                stringBuilder.Append('?');
                stringBuilder.Append(urlParamString);
            }

            return stringBuilder.ToString();
        }
    }
}