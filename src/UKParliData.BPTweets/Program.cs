using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    /// <summary>
    /// This is the main start up class for the program.
    /// </summary>

    public class Program
    {
        public static readonly int ShortenedUrlLength
            = Int32.Parse(ConfigurationManager.AppSettings["Twitter.ShortenedUrlLength"]);

        /// <summary>
        /// This is the main entry point. 
        /// </summary>
        /// <param name="args">
        /// Use the /a argument to set up the pin number given by Twitter when setting up authentication for the account.
        /// If no argument is specified program just publishes the tweets.
        /// </param>

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
                    client
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
                string tweet = String.Format("New {0}: {1} {2}",
                    bp.Type,
                    bp.Identifier,
                    bp.Title
                );

                // 139 rather than 140 to allow for space between the title and the url
                int maxLength = 139 - ShortenedUrlLength;

                if (tweet.Length > maxLength)
                {
                    tweet = tweet.Substring(0, maxLength - 3) + "...";
                }

                tweet += String.Format(
                    " http://www.parliament.uk/briefing-papers/{0}", 
                    bp.Identifier.Replace("/", "-").Replace(" ", "-")
                );

                client.Tweet(tweet);
                log.LogTweetedID(bp.Identifier);
                Console.WriteLine(tweet);
            }
        }
    }
}
