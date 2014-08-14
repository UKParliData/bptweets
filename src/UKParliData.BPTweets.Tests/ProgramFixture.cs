using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace UKParliData.BPTweets.Tests
{
    [TestFixture]
    public class ProgramFixture
    {
        private Mock<IBPReader> GetTestBPReader()
        {
            var baseReader = new BPReader(null);
            var reader = new Mock<IBPReader>();
            reader.Setup(x => x.ReadAll()).Returns(() => {
                using (var stream = FeedHelper.GetFeed())
                {
                    return baseReader.ReadAll(stream);
                }
            });

            return reader;
        }


        [Test]
        public void CanTweetAllBriefingPapers()
        {
            var tweets = new List<string>();

            var reader = GetTestBPReader();
            var client = new Mock<ITwitterClient>();
            client.Setup(x => x.Tweet(It.IsAny<string>())).Callback<string>(x => tweets.Add(x));

            var log = new Mock<ITweetLog>();
            log.Setup(x => x.GetTweetedIDs()).Returns(new HashSet<string>());

            var app = new Program(reader.Object, log.Object, client.Object);
            app.Run();

            // We should have tweeted ten times
            client.Verify(x => x.Tweet(It.IsAny<string>()), Times.Exactly(10));

            // First tweet should have the correct message
            Assert.AreEqual(
                "New Lords Library Note: Understanding and Sourcing Political Opinion Polls",
                tweets.First()
            );

            // Last tweet should have the correct message
            Assert.AreEqual(
                "New Commons Library Standard Note: Applicants and entrants to higher education: Social Indicators page",
                tweets.Last()
            );
        }


        [Test]
        public void TweetsOnlyUnsentBriefingPapers()
        {
            var tweets = new List<string>();
            var loggedIDs = new List<string>();

            var alreadyTweeted = new HashSet<string>(
                new string[] {
                    "LLN 2014/028",
                    "SN06046",
                    "SN06891",
                    "SN02815",
                    "POST PN 472"
                }
            );

            var reader = GetTestBPReader();
            var client = new Mock<ITwitterClient>();
            client.Setup(x => x.Tweet(It.IsAny<string>())).Callback<string>(x => tweets.Add(x));

            var log = new Mock<ITweetLog>();
            log.Setup(x => x.GetTweetedIDs()).Returns(alreadyTweeted);
            log.Setup(x => x.LogTweetedID(It.IsAny<string>())).Callback<string>(x => loggedIDs.Add(x));

            var app = new Program(reader.Object, log.Object, client.Object);
            app.Run();

            // We should have tweeted ten times
            client.Verify(x => x.Tweet(It.IsAny<string>()), Times.Exactly(5));

            // First tweet should have the correct message
            Assert.AreEqual(
                "New POST Note: Alternative Currencies",
                tweets.First()
            );

            // Last tweet should have the correct message
            Assert.AreEqual(
                "New Commons Library Standard Note: Applicants and entrants to higher education: Social Indicators page",
                tweets.Last()
            );

            // The new tweets need to be added to the tweet log
            log.Verify(x => x.LogTweetedID(It.IsAny<string>()), Times.Exactly(5));

            Assert.AreEqual("SN02629", loggedIDs.Last());
        }
    }
}
