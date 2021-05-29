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

        public static string spoiler<T>(T v)
        {
            return $"||{v}||";
        }

        public static async Task sendError(SocketCommandContext context, string message)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = new Color(255, 0, 0);
            e.Title = "Error";
            e.Description = message;
            await context.Channel.SendMessageAsync(embed: e.Build());
        }

        public static async Task replyError(SocketCommandContext context, string message, bool ping = false)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = new Color(255, 0, 0);
            e.Title = "Error";
            e.Description = message;
            await context.Message.ReplyAsync(embed: e.Build(), allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None));
        }

        public static async Task sendMessage(SocketCommandContext context, String text = null, Embed embed = null)
        {
            await context.Channel.SendMessageAsync(text: text, embed: embed);
        }

        public static async Task reply(SocketCommandContext context, String text = null, Embed embed = null, bool ping = false)
        {
            await context.Message.ReplyAsync(text: text, embed: embed, allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None));
        }
    }
}
