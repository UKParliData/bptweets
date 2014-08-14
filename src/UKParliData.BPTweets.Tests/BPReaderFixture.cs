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
    public class BPReaderFixture
    {
        private Stream GetFeed()
        {
            Type t = this.GetType();
            return t.Assembly.GetManifestResourceStream(t, "feed.json");
        }

        [Test]
        public void CanParseFeed()
        {
            using (var stream = GetFeed())
            {
                var briefingPapers = new BPReader().ReadAll(stream).ToList();
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
            }
        }
    }
}
