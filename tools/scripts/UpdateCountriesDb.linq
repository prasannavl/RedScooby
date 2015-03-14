<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.ComponentModel</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Runtime.Serialization</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Text.RegularExpressions</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
		string workingDir = Path.GetDirectoryName(Util.CurrentQueryPath);
		string path = Path.Combine(workingDir, "../../assets/data/CountryCodes.json");
		
		var c = new HttpClient();
		var dataString = await c.GetStringAsync("https://raw.githubusercontent.com/mledoze/countries/master/dist/countries.json");

		var data = JsonConvert.DeserializeObject<IList<CountryData>>(dataString);
		
		data = data.OrderBy(x => x.ISONumericCode).ToList();
	    var dic = new Dictionary<string, CountryEntry>();
		
		foreach (var item in data)
		{
			var ce = new CountryEntry();
			ce.IsoNumericCode = item.ISONumericCode;
			ce.CallingCodes = item.CallingCode;
			ce.Name = item.NameData.CommonName;
			ce.Iso2LetterAlphaCode = item.ISO2LetterAlphaCode;
			ce.Iso3LetterAlphaCode = item.ISO3LetterAlphaCode;
			
			dic[ce.Iso2LetterAlphaCode] = ce;
		}
		File.WriteAllText(path, JsonConvert.SerializeObject(dic));
}

[DataContract]
public class CountryData
{

	[DataMember(Name = "ccn3")]
	public int ISONumericCode {get; set;}
	[DataMember(Name = "name")]
	public NameData NameData {get; set; }
	[DataMember(Name = "cca2")]
	public string ISO2LetterAlphaCode {get; set; }
	[DataMember(Name = "cca3")]
	public string ISO3LetterAlphaCode {get; set; }
	[DataMember(Name = "callingCode")]
	public IList<string> CallingCode {get; set; }
}

[DataContract]
public class NameData {
	[DataMember(Name = "common")]
	public string CommonName {get;set; }
}

    [DataContract]
    public class CountryEntry
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