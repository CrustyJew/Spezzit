using System;
using System.Collections.Generic;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;

namespace Spezzit
{
    class Tweeter
    {
        private ITwitterCredentials creds;
        private const string message = "";
        public Tweeter(BotConfig config) {
            creds = new TwitterCredentials(config.TwitterKey, config.TwitterSecret,config.TwitterToken,config.TwitterTokenSecret);
            //creds = AuthFlow.CreateCredentialsFromVerifierCode(pin, authContext);
             
        }
        public bool SendTweet(string userData, string link ) {

            return true;
        }
    }
}
