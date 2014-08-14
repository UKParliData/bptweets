using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    public class TweetLog : ITweetLog
    {
        public ISet<string> GetTweetedIDs()
        {
            return new HashSet<string>();
        }

        public void LogTweetedID(string id)
        {
        }
    }
}
