using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    //[Group("prefix")]
    public class RemoteCommands : ModuleBase<SocketCommandContext>
    {
        [Command("source")]
        [Alias("github")]
        [Summary("Returns the link to my public repository.")]
        public async Task SourceCommand()
        {
            await Context.Channel.SendMessageAsync("https://github.com/StrengthDev/intelligence-bot");
        }
    }
}
