using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UKParliData.BPTweets
{
    public class BPReader
    {
        public IEnumerable<BriefingPaper> ReadAll(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var obj = (JObject) JToken.ReadFrom(new JsonTextReader(reader));
                return
                    from item in obj["result"]["items"]
                    select new BriefingPaper()
                    {
                        Title = (string)(item["title"].First()),
                        Description = (string)(item["description"].First()),
                        Identifier = (string)item["identifier"],
                        Date = (DateTime)(item["date"][0]["_value"])
                    };
            }
        }
    }
}
