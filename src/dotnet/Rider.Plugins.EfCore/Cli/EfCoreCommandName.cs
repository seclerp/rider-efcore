using System;

namespace Rider.Plugins.EfCore.Cli
{
    [Obsolete("Remove when all commands will be implemented on frontend")]
    public sealed class EfCoreCommandName
    {
        public string[] CommandParts { get; }

        public EfCoreCommandName(string[] commandParts)
        {
            CommandParts = commandParts;
        }
    }
}