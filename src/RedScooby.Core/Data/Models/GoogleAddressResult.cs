// Author: Prasanna V. Loganathar
// Created: 12:42 PM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RedScooby.Data.Models
{
    [DataContract]
    public class GoogleAddressResult
    {
        public const string OkStatus = "OK";

        [DataMember(Name = "results")]
        public IList<Result> Results { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataContract]
        public class Result
        {
            [DataMember(Name = "formatted_address")]
            public string FormattedAddress { get; set; }
        }
    }
}
