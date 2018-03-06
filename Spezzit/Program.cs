using Microsoft.Extensions.Configuration;
using RedditSharp;
using System;
using System.IO;

namespace Spezzit
{
    class Program
    {
        public static BotConfig Configuration = new BotConfig();
        public static BotWebAgent RedditAgent;
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("privateconfig.json", false, true);

            var config = configBuilder.Build();

            config.Bind(Configuration);

            RedditAgent = new BotWebAgent(Configuration.Username, Configuration.Password, Configuration.ClientID, Configuration.Secret, "");

            var inboxWatcher = new InboxWatcher();
            inboxWatcher.Run();


            Tweeter t = new Tweeter("", Configuration, "snoonotes");
            Console.ReadLine();

        }
    }
}
