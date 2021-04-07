using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    static class DiscordUtil
    {
        public static string bold<T>(T v)
        {
            return $"**{v}**";
        }

        public static string italic<T>(T v)
        {
            return $"*{v}*";
        }

        public static string italicBold<T>(T v)
        {
            return $"***{v}***";
        }

        public static string highlight<T>(T v)
        {
            return $"`{v}`";
        }

        public static string underline<T>(T v)
        {
            return $"__{v}__";
        }

        public static string crossout<T>(T v)
        {
            return $"~~{v}~~";
        }

        public static string code<T>(T v)
        {
            return $"```{v}```";
        }

        public static async Task sendError(SocketCommandContext context, string message)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = new Color(255, 0, 0);
            e.Title = "Error";
            e.Description = message;
            await context.Channel.SendMessageAsync(embed: e.Build());
        }
    }
}
