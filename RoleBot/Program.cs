using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;

namespace RoleBot
{
    internal static class Program
    {
        private static async Task Main()
        {
            Config config = new()
            {
                Token = "ODM5MTg4MDg4NzE4NDkxNjk5.YJGAzw.PvaAxfZzX9BelXfJvngnGAcgGn8",
                ReactionMessages = new Dictionary<ulong, ReactionMessage>
                {
                    {
                        340543173422088212, new ReactionMessage
                        {
                            RoleChannelId = 839197463134994453,
                            RoleMessageId = 839544606700863518,
                            ReactionsToRoles = new Dictionary<string, ulong>
                            {
                                { ":red_square:", 839340618257924126 },
                                { ":green_square:", 839558539843338250 },
                                { ":blue_square:", 839558705539973140 },
                            },
                        }
                    },
                    {
                        648916074179330048, new ReactionMessage
                        {
                            RoleChannelId = 838993244716138519,
                            RoleMessageId = 839659920324296715,
                            ReactionsToRoles = new Dictionary<string, ulong>
                            {
                                { ":cake:", 765618387518095390 },
                                { ":computer:", 648916964617748480 },
                                { ":art:", 765618727621886053 },
                                { ":carpentry_saw:", 652688053063778305 },
                                { ":evergreen_tree:", 657656818620825628 },
                                { ":musical_keyboard:", 692443834721894411 },
                                { ":regional_indicator_w:", 735312765551640597 },
                            },
                        }
                    },
                },
            };
            //Config config = JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync("Config.json"));

            DiscordClient client = new(new DiscordConfiguration
            {
                Token = config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
            });

            await client.ConnectAsync();
            ReactWatcher reactWatcher = new(client, config);

            await Task.Delay(-1);
        }
    }
}