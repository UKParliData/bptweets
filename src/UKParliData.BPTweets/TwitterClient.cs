using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    public class TwitterClient : ITwitterClient
    {
        public void Tweet(string tweet)
        {
            Console.WriteLine(tweet);
        }
    }
}
