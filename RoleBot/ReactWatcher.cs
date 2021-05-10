using System;
using System.Collections.Generic;
using System.Linq;
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

                foreach (ReactionMessage reactionMessage in this.config.ReactionMessages[guildId])
                {
                    foreach ((string emojiName, ulong roleId) in reactionMessage.ReactionsToRoles)
                    {
                        DiscordEmoji emoji = DiscordEmoji.FromName(client, emojiName);
                        DiscordRole role = e.Guild.GetRole(roleId);

                        reactionToRole.Add(emoji, role);
                    }
                }
            }

            foreach (ReactionMessage reactionMessage in this.config.ReactionMessages[guildId])
            {
                DiscordChannel channel = await client.GetChannelAsync(reactionMessage.RoleChannelId);
                DiscordMessage message = await channel.GetMessageAsync(reactionMessage.RoleMessageId);

                _ = Task.Run(async () =>
                {
                    foreach (DiscordEmoji emoji in reactionToRole.Keys)
                    {
                        await message.CreateReactionAsync(emoji);
                        await Task.Delay(TimeSpan.FromSeconds(0.25));
                    }
                });
            }
        }

        private async Task MessageReactionAdded(DiscordClient client, MessageReactionAddEventArgs args)
        {
            ulong guildId = args.Guild.Id;

            if (!args.User.IsBot
             && this.config.ReactionMessages[guildId].Any(e => e.RoleChannelId == args.Channel.Id)
             && this.config.ReactionMessages[guildId].Any(e => e.RoleMessageId == args.Message.Id))
            {
                DiscordMember user = await args.Guild.GetMemberAsync(args.User.Id);
                await user.GrantRoleAsync(this.guildWithReactionRoles[guildId][args.Emoji]);
            }
        }

        private async Task MessageReactionRemoved(DiscordClient client, MessageReactionRemoveEventArgs args)
        {
            ulong guildId = args.Guild.Id;

            if (!args.User.IsBot
             && this.config.ReactionMessages[guildId].Any(e => e.RoleChannelId == args.Channel.Id)
             && this.config.ReactionMessages[guildId].Any(e => e.RoleMessageId == args.Message.Id))
            {
                DiscordMember user = await args.Guild.GetMemberAsync(args.User.Id);
                await user.RevokeRoleAsync(this.guildWithReactionRoles[guildId][args.Emoji]);
            }
        }
    }
}