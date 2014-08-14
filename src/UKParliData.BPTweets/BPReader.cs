using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UKParliData.BPTweets
{
    public class BPReader : IBPReader
    {
        private string url;

        public BPReader(string url)
        {
            this.url = url;
        }


        public IEnumerable<BriefingPaper> ReadAll()
        {
            var request = WebRequest.CreateHttp(url);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return this.ReadAll(stream);
            }
        }


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
                        Date = (DateTime)(item["date"][0]["_value"]),
                        Type = (string)(item["type"]["label"]["_value"])
                    };
            }
        }
    }
}