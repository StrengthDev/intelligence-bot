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

        public static string code<T>(T v) //supported languages: https://gist.github.com/matthewzring/9f7bbfd102003963f9be7dbcf7d40e51
        {
            return $"```{v}```";
        }

        public static string spoiler<T>(T v)
        {
            return $"||{v}||";
        }

        public static string line_quote<T>(T v)
        {
            return $"> {v}";
        }

        public static string full_quote<T>(T v)
        {
            return $">>> {v}";
        }

        public static string statusString(int i)
        {
            switch (i)
            {
                case 0:
                    return "Offline"; // UserStatus.Offline;
                case 1:
                    return "Online"; // UserStatus.Online;
                case 2:
                    return "Idle"; // UserStatus.Idle;
                case 3:
                    return "Away from keyboard"; // UserStatus.AFK;
                case 4:
                    return "Do not disturb"; // UserStatus.DoNotDisturb;
                case 5:
                    return "Invisible"; // UserStatus.Invisible;
                default:
                    return "Offline"; // UserStatus.Offline;
            }
        }

        public static string activityString(int i)
        {
            switch (i)
            {
                case 0:
                    return "Playing"; // ActivityType.Playing;
                case 1:
                    return "Streaming"; // ActivityType.Streaming;
                case 2:
                    return "Listening to"; // ActivityType.Listening;
                case 3:
                    return "Watching"; // ActivityType.Watching;
                case 4:
                    return ""; // ActivityType.CustomStatus;
                default:
                    return "Playing"; // ActivityType.Playing;
            }
        }

        public static async Task sendError(SocketCommandContext context, string message)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = new Color(255, 0, 0);
            e.Title = EmojiUnicode.WARNING + "   Error";
            e.Description = message;
            await context.Channel.SendMessageAsync(embed: e.Build());
        }

        public static async Task replyError(SocketCommandContext context, string message, bool ping = false)
        {
            EmbedBuilder e = new EmbedBuilder();
            e.Color = new Color(255, 0, 0);
            e.Title = EmojiUnicode.WARNING + "   Error";
            e.Description = message;
            await context.Message.ReplyAsync(embed: e.Build(), allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None));
        }

        public static async Task sendMessage(SocketCommandContext context, string text = null, Embed embed = null)
        {
            await context.Channel.SendMessageAsync(text: text, embed: embed);
        }

        public static async Task reply(SocketCommandContext context, string text = null, Embed embed = null, bool ping = false)
        {
            await context.Message.ReplyAsync(text: text, embed: embed, allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None));
        }
    }
}
