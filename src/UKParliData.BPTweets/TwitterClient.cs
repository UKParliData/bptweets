using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using LinqToTwitter;

namespace UKParliData.BPTweets
{
    public class TwitterClient : ITwitterClient
    {
        public void Tweet(string tweet)
        {
            var auth = new PinAuthorizer() {
                CredentialStore = new InMemoryCredentialStore() {
                    ConsumerKey = ConfigurationManager.AppSettings["Twitter.ConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["Twitter.ConsumerSecret"],
                    OAuthToken = ConfigurationManager.AppSettings["Twitter.AccessToken"],
                    OAuthTokenSecret = ConfigurationManager.AppSettings["Twitter.AccessTokenSecret"]
                }
            };

            using (var ctx = new TwitterContext(auth)) {
                ctx.TweetAsync(tweet).Wait();
            }
        }


        public async Task Authorise()
        {
            var auth = new PinAuthorizer() {
                CredentialStore = new InMemoryCredentialStore() {
                    ConsumerKey = ConfigurationManager.AppSettings["Twitter.ConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["Twitter.ConsumerSecret"]
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () => {
                    Console.Write("\nEnter PIN: ");
                    return Console.ReadLine();
                }
            };

            await auth.AuthorizeAsync();

            var doc = XDocument.Load("private.config");
            var el = doc.Element("appSettings");
            el.Add(new XElement("add",
                new XAttribute("key", "Twitter.AccessToken"),
                new XAttribute("value", auth.CredentialStore.OAuthToken)
            ));
            el.Add(new XElement("add",
                new XAttribute("key", "Twitter.AccessTokenSecret"),
                new XAttribute("value", auth.CredentialStore.OAuthTokenSecret)
            ));

            doc.Save("private.config");
        }
    }
}
