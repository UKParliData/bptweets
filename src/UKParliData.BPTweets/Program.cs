using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program(
                new BPReader(),
                new TweetLog(),
                new TwitterClient()
            ).Run();
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

        void Run()
        {

        }
    }
}
