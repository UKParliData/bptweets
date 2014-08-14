using System;
using System.Collections.Generic;

namespace UKParliData.BPTweets
{
    public interface ITweetLog
    {
        ISet<string> GetTweetedIDs();

        void LogTweetedID(string id);
    }
}
