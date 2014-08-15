using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    public class Program
    {
        static void Main(string[] args)
        {
            var client = new TwitterClient();

            if (args.Contains("/a", StringComparer.OrdinalIgnoreCase))
            {
                client.Authorise().Wait();
            }
            else
            {
                new Program(
                    new BPReader(ConfigurationManager.AppSettings["BPFeedUrl"]),
                    new TweetLog("log.txt"),
                    new TwitterClient()
                ).Run();
            }
        }


        private IBPReader reader;
        private ITweetLog log;
        private ITwitterClient client;


        public Program(IBPReader reader, ITweetLog log, ITwitterClient client)
        {

            this.reader = reader;
            this.log = log;
            this.client = client;
        }

        public void Run()
        {
            var tweetedIDs = this.log.GetTweetedIDs();
            foreach (var bp in reader.ReadAll().Where(x => !tweetedIDs.Contains(x.Identifier)).OrderBy(x => x.Date))
            {
                string tweet = String.Format("New {0}: {1} {2} http://www.parliament.uk/briefing-papers/{3}",
                    bp.Type,
                    bp.Identifier,
                    bp.Title,
                    bp.Identifier.Replace("/", "-").Replace(" ", "-")
                );
                client.Tweet(tweet);
                log.LogTweetedID(bp.Identifier);
                Console.WriteLine(tweet);
            }
        }
    }
}
