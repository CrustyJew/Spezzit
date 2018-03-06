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

        public void OnCompleted() {
            throw new NotImplementedException();
        }

        public void OnError( Exception error ) {
            throw new NotImplementedException();
        }

        public void OnNext( ModAction value ) {
            if (value.ModeratorName != "AutoModerator") return;


        }

        public void Run(IWebAgent agent,string subreddit ) {
            sub = RedditSharp.Things.Subreddit.GetByNameAsync(agent, subreddit).Result;
            var stream = sub.GetModerationLog().Stream();
            stream.Enumerate(new System.Threading.CancellationToken());
        }
    }
}
