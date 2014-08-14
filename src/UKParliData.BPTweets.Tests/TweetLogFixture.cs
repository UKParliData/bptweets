using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UKParliData.BPTweets.Tests
{
    [TestFixture]
    public class TweetLogFixture
    {
        private string filename;

        [SetUp]
        public void CreateLogFile()
        {
            this.filename = Path.GetTempFileName();
        }

        [TearDown]
        public void DeleteLogFile()
        {
            File.Delete(this.filename);
        }
        
        [Test]
        public void CanLogID()
        {
            var tweetLog = new TweetLog(filename);
            tweetLog.LogTweetedID("SN01234");
            var contents = File.ReadAllLines(filename);
            Assert.AreEqual("SN01234", contents.First());
        }

        [Test]
        public void CanRetrieveLoggedIDs()
        {
            var tweetLog = new TweetLog(filename);
            tweetLog.LogTweetedID("SN01234");
            tweetLog.LogTweetedID("SN56789");

            var ids = tweetLog.GetTweetedIDs();

            CollectionAssert.AreEquivalent(
                new string[] { "SN01234", "SN56789" },
                ids
            );
        }
    }
}
