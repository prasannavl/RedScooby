// Author: Prasanna V. Loganathar
// Created: 12:42 PM 24-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RedScooby.Data.Models
{
    [DataContract]
    public class CountryInfo
    {
        [DataMember(Name = "n")]
        public string Name { get; set; }

        [DataMember(Name = "in")]
        public int IsoNumericCode { get; set; }

        [DataMember(Name = "i2")]
        public string Iso2LetterAlphaCode { get; set; }

        [DataMember(Name = "i3")]
        public string Iso3LetterAlphaCode { get; set; }

        [DataMember(Name = "pc")]
        public IList<string> CallingCodes { get; set; }
    }
}
