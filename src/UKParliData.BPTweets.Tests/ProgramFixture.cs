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
            var baseReader = new BPReader();
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
            var reader = GetTestBPReader();
            var client = new Mock<ITwitterClient>();

            var log = new Mock<ITweetLog>();
            log.Setup(x => x.GetTweetedIDs()).Returns(new HashSet<string>());

            var app = new Program(reader.Object, log.Object, client.Object);
            app.Run();

            // We should have tweeted ten times
            client.Verify(x => x.Tweet(It.IsAny<string>()), Times.Exactly(10));
        }
    }
}
