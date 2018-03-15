using System;
using System.Collections.Generic;
using System.Text;
using RedditSharp;
using RedditSharp.Things;

namespace Spezzit
{
    class InboxWatcher : IObserver<RedditSharp.Things.PrivateMessage>
    {
        private Reddit redditInst;
        public void OnCompleted() {
            throw new NotImplementedException();
        }

        public void OnError( Exception error ) {
            throw new NotImplementedException();
        }

        public void OnNext( PrivateMessage value ) {
            if (!value.Unread) {
                return;
            }

            if (value.IsComment || !value.Subject.StartsWith("invitation to moderate")) {
                value.SetAsReadAsync().Wait();
                return;
            }

            string subname = value.Subject.Replace("invitation to moderate /r/", "");
            var sub = RedditSharp.Things.Subreddit.GetByNameAsync(Program.RedditAgent, subname).Result;
            sub.AcceptModeratorInviteAsync().Wait();
            Program.CheckLoggers();
            value.SetAsReadAsync().Wait();
        }

        public void Run() {
            redditInst = new Reddit(Program.RedditAgent,true);
            var stream = redditInst.User.GetInbox().Stream();
            stream.Subscribe(this);
            stream.Enumerate(new System.Threading.CancellationToken());
        }
    }
}
