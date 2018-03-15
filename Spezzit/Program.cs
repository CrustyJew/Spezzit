using Microsoft.Extensions.Configuration;
using RedditSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spezzit
{
    class Program
    {
        public static BotConfig Configuration = new BotConfig();
        public static BotWebAgent RedditAgent;
        public static Reddit SpezzitBotReddit;
        public static Dictionary<string,LogWatcher> activeLoggers = new Dictionary<string,LogWatcher>();
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("privateconfig.json", false, true);

            var config = configBuilder.Build();

            config.Bind(Configuration);

            RedditAgent = new BotWebAgent(Configuration.Username, Configuration.Password, Configuration.ClientID, Configuration.Secret, "");
            SpezzitBotReddit = new Reddit(RedditAgent, true);

            var inboxWatcher = new InboxWatcher();
            inboxWatcher.Run();

            CheckLoggers();

            Console.ReadLine();

        }

        public static void CheckLoggers() {
            SpezzitBotReddit.User.GetModeratorSubreddits().ForEachAsync(sub => {
                if(!activeLoggers.ContainsKey(sub.Name)) {
                    Tweeter t = new Tweeter(Configuration);
                    LogWatcher w = new LogWatcher();
                    w.Run(RedditAgent, sub.Name, t);
                    activeLoggers.Add(sub.Name, w);
                }
            }).Wait();
        }
    }
}
