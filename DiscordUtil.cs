using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace intelligence_bot
{
    static class DiscordUtil
    {
        public static string sanitise(string text)
        {
            return text.Replace("*", "").Replace("_", "").Replace("`", "").Replace("|", "").Replace("~", "").Trim();
        }

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

        public enum date_format
        {
            DAY,            // dd/mm/yyyy
            DAY_EXTENDED,   // Month dd, yyyy
            DAY_N_TIME,     // Month dd, yyyy hh:mm
            DAY_COMPLETE,   // Weekday, Month dd, yyyy hh:mm
            TIME,           // hh:mm
            TIME_EXTENDED,  // hh:mm:ss
            RELATIVE,
        }

        public static string dynamic_date(DateTime date, date_format format)
        {
            char f = 'd';
            switch (format) 
            {
                case date_format.DAY:           f = 'd'; break;
                case date_format.DAY_EXTENDED:  f = 'D'; break;
                case date_format.DAY_N_TIME:    f = 'f'; break;
                case date_format.DAY_COMPLETE:  f = 'F'; break;
                case date_format.TIME:          f = 't'; break;
                case date_format.TIME_EXTENDED: f = 'T'; break;
                case date_format.RELATIVE:      f = 'R'; break;
            }
            return $"<t:{((DateTimeOffset)date).ToUnixTimeSeconds()}:{f}>";
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

        public static async Task sendFile(SocketCommandContext context, string path, string caption = null, bool spoiler = false)
        {
            await context.Channel.SendFileAsync(path, caption, isSpoiler: spoiler);
        }

        public static async Task replyFile(SocketCommandContext context, string path, string caption = null, bool spoiler = false, bool ping = false)
        {
            await context.Channel.SendFileAsync(path, caption, isSpoiler: spoiler, allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None), messageReference: new MessageReference(context.Message.Id));
        }

        public static async Task sendFile(SocketCommandContext context, Stream stream, string name, string caption = null, bool spoiler = false)
        {
            await context.Channel.SendFileAsync(stream, name, caption, isSpoiler: spoiler);
        }

        public static async Task replyFile(SocketCommandContext context, Stream stream, string name, string caption = null, bool spoiler = false, bool ping = false)
        {
            await context.Channel.SendFileAsync(stream, name, caption, isSpoiler: spoiler, allowedMentions: ping ? null : new AllowedMentions(AllowedMentionTypes.None), messageReference: new MessageReference(context.Message.Id));
        }
    }
}
