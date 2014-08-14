using System;
using System.Linq;
using NUnit.Framework;

namespace UKParliData.BPTweets.Tests
{
    [TestFixture]
    public class BPReaderFixture
    {
        [Test]
        public void CanParseFeed()
        {
            using (var stream = FeedHelper.GetFeed())
            {
                var briefingPapers = new BPReader(null).ReadAll(stream).ToList();
                Assert.AreEqual(10, briefingPapers.Count);
                var first = briefingPapers[0];
                Assert.AreEqual(
                    "Applicants and entrants to higher education: Social Indicators page",
                    first.Title
                );
                Assert.AreEqual(
                    "Brief snapshot of recent changes in entrants to HE via UCAS",
                    first.Description
                );
                Assert.AreEqual("SN02629", first.Identifier);
                Assert.Less(
                    new DateTime(2014, 8, 11, 11, 18, 31, 108) - first.Date,
                    TimeSpan.FromMilliseconds(1)
                );

                Assert.AreEqual(
                    "Commons Library Standard Note",
                    first.Type
                );
            }
        }
    }
}
