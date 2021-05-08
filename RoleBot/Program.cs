using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;

namespace RoleBot
{
    internal static class Program
    {
        private static async Task Main()
        {
            Config config = JsonConvert.DeserializeObject<Config>(await File.ReadAllTextAsync("Config.json"));

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