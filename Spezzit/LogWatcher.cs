using System;
using System.Collections.Generic;
using System.Text;
using RedditSharp;
using RedditSharp.Things;

namespace Spezzit
{
    class LogWatcher : IObserver<RedditSharp.Things.ModAction>
    {
        private RedditSharp.Things.Subreddit sub;
        private Tweeter subTweeter;

        public void OnCompleted() {
            throw new NotImplementedException();
        }

        public void OnError( Exception error ) {
            throw new NotImplementedException();
        }

        public void OnNext( ModAction value ) {
            if (value.ModeratorName == "AutoModerator" && value.Details.ToLower().Contains("spez_that")){
                bool isPost = value.TargetThingFullname.StartsWith("t3_");
                string reason = value.Details.Substring(value.Details.ToLower().IndexOf("spez_that") + 10);
                subTweeter.SendTweet(sub.Name, (isPost ? "post" : "comment"), value.TargetAuthorName, reason, (isPost ? value.TargetTitle : value.TargetBody), value.TargetThingPermalink);
            }
            else if  (value.Action == ModActionType.Distinguish && value.TargetThingFullname.StartsWith("t1_") && value.TargetBody.ToLower().Contains("spez_that")) {
                var modcomment = (ModeratableThing) value.GetTargetThing().Result;
                string reason = "";
                ModeratableThing thing = null;
                if(modcomment.Kind == "t4") {
                    var pm = (PrivateMessage) modcomment;
                    thing = pm.GetParent() as ModeratableThing;
                    reason = pm.Body.Substring(pm.Body.ToLower().IndexOf("spez_that") + 10);
                }
                else {
                    var c = (Comment) modcomment;
                    thing = new Reddit(sub.WebAgent, false).GetThingByFullnameAsync(c.ParentId).Result as ModeratableThing;
                    c.RemoveAsync().Wait();
                    reason = c.Body.Substring(c.Body.ToLower().IndexOf("spez_that") + 10);
                }
                string type = "thing";
                string message = "";
                string url = "";
                switch(thing.Kind) {
                    case "t3":
                        type = "post";
                        message = ((Post) thing).Title;
                        url = ((Post) thing).Permalink.ToString();
                        break;
                    case "t1":
                        type = "comment";
                        message = ((Comment) thing).Body;
                        url = ((Comment) thing).Permalink.ToString();
                        break;
                    case "t4":
                        type = "message";
                        message = ((PrivateMessage) thing).Body;
                        url = "";
                        break;
                }
                if(!string.IsNullOrWhiteSpace(url) && !url.ToLower().StartsWith("https://reddit.com/")) {
                    url = "https://reddit.com" + (url.StartsWith("/") ? "" : "/") + url;
                }
                subTweeter.SendTweet(sub.Name, type, thing.AuthorName, reason, message, url );
            }
        }

        public void Run(IWebAgent agent,string subreddit, Tweeter tweeter ) {
            sub = RedditSharp.Things.Subreddit.GetByNameAsync(agent, subreddit).Result;
            subTweeter = tweeter;
            var stream = sub.GetModerationLog().Stream();
            stream.Subscribe(this);
            stream.Enumerate(new System.Threading.CancellationToken());
        }
    }
}
