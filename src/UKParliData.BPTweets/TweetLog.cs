using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets
{
    public class TweetLog : ITweetLog
    {
        private string filename;

        public TweetLog(string filename)
        {
            this.filename = filename;
            if (!File.Exists(this.filename))
            {
                using (var f = File.Create(this.filename)) { }
            }
        }

        public ISet<string> GetTweetedIDs()
        {
            return new HashSet<string>(File.ReadAllLines(filename));
        }

        public void LogTweetedID(string id)
        {
            File.AppendAllLines(filename, new string[] { id });
        }
    }
}
