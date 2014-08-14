using System;

namespace UKParliData.BPTweets
{
    public interface ITwitterClient
    {
        void Tweet(string tweet);
    }
}
