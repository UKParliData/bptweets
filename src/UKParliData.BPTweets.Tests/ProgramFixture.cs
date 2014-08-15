using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                "New Lords Library Note: LLN 2014/028 Understanding and Sourcing Political Opinion Polls http://www.parliament.uk/briefing-papers/LLN-2014-028",
                tweets.First()
            );

            // Last tweet should have the correct message
            Assert.AreEqual(
                "New Commons Library Standard Note: SN02629 Applicants and entrants to higher education: Social Indicators page http://www.parliament.uk/briefing-papers/SN02629",
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
                "New POST Note: POST PN 475 Alternative Currencies http://www.parliament.uk/briefing-papers/POST-PN-475",
                tweets.First()
            );

            // Last tweet should have the correct message
            Assert.AreEqual(
                "New Commons Library Standard Note: SN02629 Applicants and entrants to higher education: Social Indicators page http://www.parliament.uk/briefing-papers/SN02629",
                tweets.Last()
            );

            // The new tweets need to be added to the tweet log
            log.Verify(x => x.LogTweetedID(It.IsAny<string>()), Times.Exactly(5));

            Assert.AreEqual("SN02629", loggedIDs.Last());
        }

        [Test]
        public void TruncatesTitleWhenTooLongForTweet()
        {
            var bp = new BriefingPaper() {
                Date = new DateTime(2014, 1, 1, 12, 0, 0),
                Description = "Lords Library Note",
                Identifier = "LLN 2014/029",
                Title = "The Impact of Personal Indebtedness in United Kingdom Households, Especially on Children",
                Type = "Lords Library Note"
            };

            string tweetText = null;

            var reader = new Mock<IBPReader>();
            reader.Setup(x => x.ReadAll()).Returns(new BriefingPaper[] { bp });

            var client = new Mock<ITwitterClient>();
            client.Setup(x => x.Tweet(It.IsAny<string>())).Callback<string>(x => tweetText = x);

            var log = new Mock<ITweetLog>();
            log.Setup(x => x.GetTweetedIDs()).Returns(new HashSet<string>());

            var app = new Program(reader.Object, log.Object, client.Object);
            app.Run();

            client.Verify(x => x.Tweet(It.IsAny<string>()), Times.Once);

            tweetText = Regex.Replace(tweetText, "http://www.parliament.uk/.*$", String.Empty);

            Assert.LessOrEqual(tweetText.Length + Program.ShortenedUrlLength, 140);
        }
    }
}
