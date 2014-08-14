using System;
using System.Collections.Generic;

namespace UKParliData.BPTweets
{
    public interface IBPReader
    {
        IEnumerable<BriefingPaper> ReadAll();
    }
}
