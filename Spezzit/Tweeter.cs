using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace Spezzit
{
    class Tweeter
    {
        private ITwitterCredentials creds;
        private const string message = "@reddit {sub} is alerting you to a {submission_type} by '{user}' for '{reason}'! : '{submission_text}' {url}";
        private const int static_message_length = 75; //includes url length
        public Tweeter(BotConfig config) {
            creds = new TwitterCredentials(config.TwitterKey, config.TwitterSecret,config.TwitterToken,config.TwitterTokenSecret);
            //creds = AuthFlow.CreateCredentialsFromVerifierCode(pin, authContext);
        }
        public Task SendTweet(string subreddit, string submissiontype, string user, string reason, string submissionText, string url ) {
            int maxTextLength = 280 - static_message_length - submissiontype.Length - subreddit.Length - user.Length - reason.Length;
            if (submissionText.Length > maxTextLength) {
                submissionText = submissionText.Substring(0, maxTextLength - 3) + "...";
            }
            string tweet = message.Replace("{sub}", subreddit).Replace("{submission_type}", submissiontype).Replace("{user}", user).Replace("{reason}",reason).Replace("{submission_text}", submissionText).Replace("{url}", url);
            Auth.ExecuteOperationWithCredentials(creds, () => {
                return TweetAsync.PublishTweet(tweet);
            });
            return Task.FromResult<int>(1);
        }
    }
}
