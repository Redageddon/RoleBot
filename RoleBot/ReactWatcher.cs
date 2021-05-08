using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace RoleBot
{
    public class ReactWatcher
    {
        private readonly Config config;

        private readonly Dictionary<ulong, Dictionary<DiscordEmoji, DiscordRole>> guildWithReactionRoles = new();

        public ReactWatcher(DiscordClient client, Config config)
        {
            this.config = config;
            client.GuildAvailable += this.ClientOnGuildAvailable;
            client.MessageReactionAdded += this.MessageReactionAdded;
            client.MessageReactionRemoved += this.MessageReactionRemoved;
        }

        private async Task ClientOnGuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            ulong guildId = e.Guild.Id;

            if (!this.guildWithReactionRoles.TryGetValue(guildId, out Dictionary<DiscordEmoji, DiscordRole> reactionToRole))
            {
                reactionToRole = new Dictionary<DiscordEmoji, DiscordRole>();
                this.guildWithReactionRoles.Add(guildId, reactionToRole);

                foreach ((string emojiName, ulong roleId) in this.config.ReactionMessages[guildId].ReactionsToRoles)
                {
                    DiscordEmoji emoji = DiscordEmoji.FromName(client, emojiName);
                    DiscordRole role = e.Guild.GetRole(roleId);

                    reactionToRole.Add(emoji, role);
                }
            }

            DiscordChannel channel = await client.GetChannelAsync(this.config.ReactionMessages[guildId].RoleChannelId);
            DiscordMessage message = await channel.GetMessageAsync(this.config.ReactionMessages[guildId].RoleMessageId);

            _ = Task.Run(async () =>
            {
                foreach (DiscordEmoji emoji in reactionToRole.Keys)
                {
                    await message.CreateReactionAsync(emoji);
                    await Task.Delay(TimeSpan.FromSeconds(0.25));
                }
            });
        }

        private async Task MessageReactionAdded(DiscordClient client, MessageReactionAddEventArgs args)
        {
            ulong guildId = args.Guild.Id;

            if (!args.User.IsBot
             && args.Channel.Id == this.config.ReactionMessages[guildId].RoleChannelId
             && args.Message.Id == this.config.ReactionMessages[guildId].RoleMessageId)
            {
                DiscordMember user = await args.Guild.GetMemberAsync(args.User.Id);
                await user.GrantRoleAsync(this.guildWithReactionRoles[guildId][args.Emoji]);
            }
        }

        private async Task MessageReactionRemoved(DiscordClient client, MessageReactionRemoveEventArgs args)
        {
            ulong guildId = args.Guild.Id;

            if (!args.User.IsBot
             && args.Channel.Id == this.config.ReactionMessages[guildId].RoleChannelId
             && args.Message.Id == this.config.ReactionMessages[guildId].RoleMessageId)
            {
                DiscordMember user = await args.Guild.GetMemberAsync(args.User.Id);
                await user.RevokeRoleAsync(this.guildWithReactionRoles[guildId][args.Emoji]);
            }
        }
    }
}