using System.Collections.Generic;

namespace RoleBot
{
    public struct Config
    {
        public string Token { get; set; }

        public Dictionary<ulong, ReactionMessage> ReactionMessages { get; set; }
    }
}