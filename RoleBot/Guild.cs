using System.Collections.Generic;

namespace RoleBot
{
    public struct ReactionMessage
    {
        public ulong RoleChannelId { get; set; }

        public ulong RoleMessageId { get; set; }

        public Dictionary<string, ulong> ReactionsToRoles { get; set; }
    }
}