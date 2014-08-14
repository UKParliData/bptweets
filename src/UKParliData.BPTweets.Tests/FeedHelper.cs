using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UKParliData.BPTweets.Tests
{
    public static class FeedHelper
    {
        internal static Stream GetFeed()
        {
            Type t = typeof(FeedHelper);
            return t.Assembly.GetManifestResourceStream(t, "feed.json");
        }

    }
}
