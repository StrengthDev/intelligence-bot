using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace intelligence_bot
{
    class Command
    {
        public virtual void execute(EventHandler data) { }
    }

    #region Local
    class LServersCommand : Command
    {
        public LServersCommand() { }

        public override void execute(EventHandler data)
        {
            data.servers = data.socket.Guilds.ToArray();
            int i = 0;
            foreach(SocketGuild server in data.servers)
            {
                Console.WriteLine($"{i}. Name: {server.Name} ; ID: {server.Id}");
                i++;
            }
        }
    }

    class LChannelsCommand : Command
    {
        public LChannelsCommand() { }

        public override void execute(EventHandler data)
        {
            if (data.currentServer == -1)
            {
                Console.WriteLine("No server selected.");
                return;
            }
            data.channels = data.servers[data.currentServer].TextChannels.ToArray();
            int i = 0;
            foreach (SocketTextChannel channel in data.channels)
            {
                Console.WriteLine($"{i}. Name: {channel.Name} ; ID: {channel.Id}");
                i++;
            }
        }
    }

    class SelectServerCommand : Command
    {
        private int index;

        public SelectServerCommand(int index)
        {
            this.index = index;
        }

        public override void execute(EventHandler data)
        {
            if(data.servers == null)
            {
                Console.WriteLine("Please list servers atleast once.");
                return;
            }
            data.currentServer = index;
            data.currentChannel = -1;
            Console.WriteLine($"Selected server: {data.servers[index].Name}");
        }
    }

    class SelectChannelCommand : Command
    {
        private int index;

        public SelectChannelCommand(int index)
        {
            this.index = index;
        }

        public override void execute(EventHandler data)
        {
            if(data.currentServer == -1)
            {
                Console.WriteLine("No server selected.");
                return;
            }
            data.currentChannel = index;
            Console.WriteLine($"Selected channel: {data.channels[index].Name}");
        }
    }

    class CurrentCommand : Command
    {

    }

    class SayCommand : Command
    {
        private string message;

        public SayCommand(string message)
        {
            this.message = message;
        }

        public override void execute(EventHandler data)
        {
            if (data.currentServer == -1 || data.currentChannel == -1)
            {
                Console.WriteLine("No server or channel selected.");
                return;
            }
            data.channels[data.currentChannel].SendMessageAsync(message).Wait();
        }
    }

    class SetStatusCommand : Command
    {
        private string status;
        private int type;

        public SetStatusCommand(string status, int type = 0)
        {
            this.status = status;
            this.type = type == 1 ? 0 : type;
        }

        public override void execute(EventHandler data)
        {
            data.socket.SetActivityAsync(new Game(status, (ActivityType)type)).Wait();
            Console.WriteLine($"Status set to: {DiscordUtil.activityString(type)} {status}");
        }
    }
    #endregion

    #region Generics
    class MessageCommand : Command
    {
        private SocketCommandContext context;
        private string message;
        private Embed embed;

        public MessageCommand(SocketCommandContext context, string message = null, Embed embed = null)
        {
            this.context = context;
            this.message = message;
            this.embed = embed;
        }

        public override void execute(EventHandler data)
        {
            DiscordUtil.sendMessage(context, message, embed).Wait();
        }
    }

    class ReplyCommand : Command
    {
        private SocketCommandContext context;
        private string message;
        private Embed embed;

        public ReplyCommand(SocketCommandContext context, string message = null, Embed embed = null)
        {
            this.context = context;
            this.message = message;
            this.embed = embed;
        }

        public override void execute(EventHandler data)
        {
            DiscordUtil.reply(context, message, embed, true).Wait();
        }
    }
    #endregion

    #region Miscellaneous
    class SourceCommand : Command
    {
        private SocketCommandContext context;

        public SourceCommand(SocketCommandContext context)
        {
            this.context = context;
        }

        public override void execute(EventHandler data)
        {
            string url;
            data.config.TryGetValue(ConfigKeyword.SOURCE_LINK, out url);
            DiscordUtil.reply(context, url).Wait();
        }
    }

    class HelpCommand : Command
    {
        private SocketCommandContext context;

        public HelpCommand(SocketCommandContext context)
        {
            this.context = context;
        }

        public override void execute(EventHandler data)
        {
            string sarrow = DiscordUtil.bold(">>");
            string smin = DiscordUtil.highlight("min");
            string smax = DiscordUtil.highlight("max");
            string sn = DiscordUtil.highlight("n");
            string sx = DiscordUtil.highlight("x");
            string starget = DiscordUtil.highlight("target");
            string exp = DiscordUtil.highlight("expression");
            string msg = DiscordUtil.highlight("message");
            string smj = DiscordUtil.highlight("emoji");


            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = EmojiUnicode.SCROLL + "   " + DiscordUtil.italic("Help");
            embed.Description = "";
            embed.Color = new Color(15, 150, 255);


            EmbedFieldBuilder rng = new EmbedFieldBuilder();
            string sroll = $"~roll [{smax}] [{sn}] {sarrow} Roll a number between 1 and {smax} (inclusive), {sn} times. ({sn} defaults to 1)\n";
            string srand = $"~rand [{smin}] [{smax}] {sarrow} Generates a random number between {smin} (inclusive) and {smax} (exclusive).\n";
            string spick = $"~pick [{sn}] [{smax}] {sarrow} Randomly select {sn} unique elements out of a range between 1 and {smax} (inclusive).\n";
            rng.Name = "Random Generators";
            rng.Value = sroll + srand + spick;
            rng.IsInline = false;
            embed.AddField(rng);
            
            EmbedFieldBuilder math = new EmbedFieldBuilder();
            string srng = $"~rng [{sx}] [{sn}] {sarrow} Calculate the probability of an event with chance {sx}, to happen at least once in {sn} attempts.\n";
            string schance = $"~chance [{sx}] [{starget}] {sarrow} Calculate the number of attempts necessary for an event with chance {sx}, to reach a statistical chance {starget}.\n";
            string scalc = $"[{exp}] {sarrow} Calculate the value of the expression.\n";
            math.Name = "Math";
            math.Value = srng + schance + scalc;
            math.IsInline = false;
            embed.AddField(math);

            EmbedFieldBuilder misc = new EmbedFieldBuilder();
            string ssource = $"~source {sarrow} Display the link to this bot's original source code.\n";
            string stimer = $"~timer [{sn}] [{msg}] {sarrow} After {sn} minutes have passed, the bot will ping the user and display an optional {msg}.\n";
            string semoji = $"~emoji [{smj}] {sarrow} Display the unicode of {smj}.\n";
            misc.Name = "Miscellaneous";
            misc.Value = ssource + stimer + semoji;
            misc.IsInline = false;
            embed.AddField(misc);
            
            DiscordUtil.reply(context, embed: embed.Build()).Wait();
        }
    }

    class ProfileCommand : Command
    {
        private SocketCommandContext context;
        private SocketUser user;

        public ProfileCommand(SocketCommandContext context, SocketUser user)
        {
            this.context = context;
            this.user = user;
        }

        public override void execute(EventHandler data)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = user.ToString();

            embed.Description = 
                DiscordUtil.bold("UUID: ") + user.Id + '\n' + 
                DiscordUtil.bold("Status: ") + DiscordUtil.statusString((int)user.Status) + '\n' + 
                DiscordUtil.bold("Is a bot: ") + user.IsBot + '\n';
            
            string url = user.GetAvatarUrl();
            if(url.Contains('?'))
            {
                url = url.Substring(0, url.IndexOf('?')) + "?size=512";
            }
            embed.ImageUrl = url;

            embed.Color = new Color(110, 40, 255);

            foreach(IActivity a in user.Activities)
            {
                EmbedFieldBuilder field = new EmbedFieldBuilder();
                field.Name = a.Type == ActivityType.CustomStatus ? "Mood:" : DiscordUtil.activityString((int)a.Type) + ':';
                field.Value = a.ToString();
                field.IsInline = true;
                embed.AddField(field);
            }

            DiscordUtil.reply(context, embed: embed.Build()).Wait();
        }
    }

    class TimerCommand : Command
    {
        private SocketCommandContext context;
        private int minutes;
        private string message;

        public TimerCommand(SocketCommandContext context, int minutes, string message)
        {
            this.context = context;
            this.minutes = minutes;
            this.message = message;
        }

        public override void execute(EventHandler data)
        {
            string tm = message != null && message.Trim() != "" ? DiscordUtil.bold("Beep Beep: ") + DiscordUtil.highlight(message.Replace("*", "").Replace("_", "").Replace("`", "").Replace("|", "").Replace("~", "").Trim()) : DiscordUtil.bold("Beep Beep.");
            Task.Run(() => {
                TimeSpan time = new TimeSpan(0, minutes, 0);
                Thread.Sleep(time);
                data.queue.Add(new CommandEvent(new ReplyCommand(context, tm)));
            });
            context.Message.AddReactionAsync(new Emoji(EmojiUnicode.TIMER));
        }
    }

    class AskCommand : Command
    {
        private SocketCommandContext context;
        private string question;

        public AskCommand(SocketCommandContext context, string question)
        {
            this.context = context;
            this.question = question;
        }

        public override void execute(EventHandler data)
        {
            if (data.answers != null)
            {
                uint roll = BitConverter.ToUInt32(BitConverter.GetBytes(question.GetHashCode()), 0) % data.answers[data.answers.Length - 1].Item1;
                string answer = "I do not know.";
                foreach (Tuple<uint, string> a in data.answers)
                {
                    if (roll < a.Item1)
                    {
                        answer = a.Item2;
                        break;
                    }
                }
                DiscordUtil.reply(context, answer).Wait();
            }
            else
            {
                DiscordUtil.replyError(context, "Command unavailable.").Wait();
            }
        }
    }

    class PowerLevelCommand : Command
    {
        private SocketCommandContext context;

        public PowerLevelCommand(SocketCommandContext context)
        {
            this.context = context;
        }

        public override void execute(EventHandler data)
        {
            if (data.answers != null)
            {
                uint roll = (uint)((context.User.Id % data.plf_length) + 1);
                string answer = data.plf0;
                for (uint i = 0; i < roll; i++)
                {
                    answer += data.plf1;
                }
                answer += data.plf2;
                DiscordUtil.reply(context, answer).Wait();
            }
            else
            {
                DiscordUtil.replyError(context, "Command unavailable.").Wait();
            }
        }
    }

    class EmojiCommand : Command
    {
        private SocketCommandContext context;
        private string emoji;

        public EmojiCommand(SocketCommandContext context, string emoji)
        {
            this.context = context;
            this.emoji = emoji;
        }

        public override void execute(EventHandler data)
        {
            string unicode = string.Format("\\U{0:X8}", char.ConvertToUtf32(emoji, 0));
            DiscordUtil.reply(context, DiscordUtil.bold(unicode)).Wait();
        }
    }

    class GenerateCommand : Command
    {
        private SocketCommandContext context;
        private string generator;

        public GenerateCommand(SocketCommandContext context, string generator)
        {
            this.context = context;
            this.generator = generator;
        }

        public override void execute(EventHandler data)
        {
            if(generator == "r")
            {
                DiscordUtil.replyFile(context, "img.png").Wait();
            } else
            {
                DiscordUtil.sendFile(context, "img.png").Wait();
            }
        }
    }
    #endregion

    #region Math
    class RollCommand : Command
    {
        private SocketCommandContext context;
        private int max;
        private int n;

        public RollCommand(SocketCommandContext context, int max, int n)
        {
            this.context = context;
            this.max = max;
            this.n = n;
        }

        public override void execute(EventHandler data)
        {
            int x = data.rng.Next(1, max + 1);
            string message = x.ToString();
            for (int i = 1; i < n; i++)
            {
                message += $" {data.rng.Next(1, max + 1)}";
            }
            DiscordUtil.reply(context, DiscordUtil.code(message)).Wait();
        }
    }

    class RandCommand : Command
    {
        private SocketCommandContext context;
        private int min;
        private int max;

        public RandCommand(SocketCommandContext context, int min, int max)
        {
            this.context = context;
            this.min = min;
            this.max = max;
        }

        public override void execute(EventHandler data)
        {
            DiscordUtil.reply(context, DiscordUtil.bold(data.rng.Next(min, max))).Wait();
        }
    }

    class RngCommand : Command
    {
        private SocketCommandContext context;
        private float x;
        private int n;

        public RngCommand(SocketCommandContext context, float x, int n)
        {
            this.context = context;
            this.x = x;
            this.n = n;
        }

        public override void execute(EventHandler data)
        {
            float chance = (1.0f - (float)Math.Pow(1.0f - x, n)) * 100.0f;
            string cs = string.Format("{0,0:0.###}%", chance);
            DiscordUtil.reply(context, $"{DiscordUtil.bold(cs)} chance for an occurrence, with an absolute chance of {x * 100.0f}%, to happen at least once after {n} attempts.").Wait();
        }
    }

    class PickCommand : Command
    {
        private SocketCommandContext context;
        private int n;
        private int max;

        public PickCommand(SocketCommandContext context, int n, int max)
        {
            this.context = context;
            this.n = n;
            this.max = max;
        }

        public override void execute(EventHandler data)
        {
            HashSet<int> set = new HashSet<int>();
            int x = data.rng.Next(1, max + 1);
            set.Add(x);
            string message = x.ToString();
            while (set.Count() < n)
            {
                x = data.rng.Next(1, max + 1);
                if(set.Add(x))
                {
                    message += $" {x}";
                }
            }
            DiscordUtil.reply(context, DiscordUtil.code(message)).Wait();
        }
    }

    class ChanceCommand : Command
    {
        private SocketCommandContext context;
        private float x;
        private float t;

        public ChanceCommand(SocketCommandContext context, float x, float t)
        {
            this.context = context;
            this.x = x;
            this.t = t;
        }

        public override void execute(EventHandler data)
        {
            double fattempts = Math.Log(1.0f - t) / Math.Log(1.0f - x);
            int attempts = (int)Math.Ceiling(fattempts);
            float chance = (1.0f - (float)Math.Pow(1.0f - x, attempts)) * 100.0f;
            string cs = DiscordUtil.bold(string.Format("{0,0:0.###}%", chance));
            string message = $"Within {DiscordUtil.bold(attempts)} attempts, an occurence with {x * 100.0f}% absolute chance, reaches a statistical chance of about {cs}.";
            DiscordUtil.reply(context, message).Wait();
        }
    }
    #endregion

    #region Text
    class RegexReplaceCommand : Command
    {
        private SocketCommandContext context;
        private string args;

        public RegexReplaceCommand(SocketCommandContext context, string args)
        {
            this.context = context;
            this.args = args;
        }

        public override void execute(EventHandler data)
        {
            string trimmed = args.TrimStart();
            Match regex = Regex.Match(trimmed, "/(.+)/([igmuys]*) \"(.*)\"");
            if(regex.Success)
            {
                string flag_s = regex.Groups[2].Value;
                RegexOptions options = RegexOptions.None;
                foreach(char c in flag_s)
                {
                    switch(c)
                    {
                        case 'i':
                            options |= RegexOptions.IgnoreCase;
                            break;
                        case 'm':
                            options |= RegexOptions.Multiline;
                            break;
                        default:
                            break;
                    }
                }
                string replacement = regex.Groups[3].Value;//args.Substring(regex.Length);
                var messages = context.Channel.GetMessagesAsync(2).FlattenAsync().Result;
                //DiscordUtil.reply(context, $"Message: {messages.Last().Content}\nRegex: {regex.Groups[1].Value}\nFlag: {flag}\nReplacement: {replacement}\n").Wait();
                try
                {
                    DiscordUtil.reply(context, Regex.Replace(messages.Last().Content, regex.Groups[1].Value, replacement, options)).Wait();
                } catch (Exception)
                {
                    DiscordUtil.replyError(context, "Regular expression or replacement string invalid.").Wait();
                }
            } else
            {
                DiscordUtil.replyError(context, "Could not parse regex.").Wait();
            }
        }
    }

    #endregion

    #region Games
    class GameSListCommand : Command
    {
        private SocketCommandContext context;

        public GameSListCommand(SocketCommandContext context)
        {
            this.context = context;
        }

        public override void execute(EventHandler data)
        {
            string[] games = data.db.listGames();
            string message = "Available games:\n";
            foreach (string game in games)
            {
                message += game;
                message += "\t";
            }
            message = DiscordUtil.code(message);
            DiscordUtil.reply(context, message).Wait();
        }
    }

    class GamePListCommand : Command
    {
        private SocketCommandContext context;
        SocketUser user;

        public GamePListCommand(SocketCommandContext context, SocketUser user)
        {
            this.context = context;
            this.user = user;
        }

        public override void execute(EventHandler data)
        {
            if(!data.db.playerExists(user.Id))
            {
                DiscordUtil.reply(context, "User has no games.").Wait();
                return;
            }
            string[] games = data.db.listGames(user.Id);
            string message = user.Username + "'s games:\n";
            foreach(string game in games)
            {
                message += game;
                message += "\t";
            }
            message = DiscordUtil.code(message);
            DiscordUtil.reply(context, message).Wait();
        }
    }

    class GameIntersectionCommand : Command
    {
        private SocketCommandContext context;
        SocketUser[] users;

        public GameIntersectionCommand(SocketCommandContext context, SocketUser[] users)
        {
            this.context = context;
            this.users = users;
        }

        public override void execute(EventHandler data)
        {
            List<ulong> ids = new List<ulong>();
            foreach(SocketUser user in users)
            {
                if (!data.db.playerExists(user.Id))
                {
                    DiscordUtil.reply(context, "User has no games.").Wait();
                    return;
                }
                ids.Add(user.Id);
            }
            string[] games = data.db.intersect(ids.ToArray());
            string message = "Intersection:\n";
            foreach (string game in games)
            {
                message += game;
                message += "\t";
            }
            message = DiscordUtil.code(message);
            DiscordUtil.reply(context, message).Wait();
        }
    }

    class GameAddCommand : Command
    {
        private SocketCommandContext context;
        private string game;

        public GameAddCommand(SocketCommandContext context, string game)
        {
            this.context = context;
            this.game = game;
        }

        public override void execute(EventHandler data)
        {
            if(data.db.gameExists(game))
            {
                DiscordUtil.replyError(context, "Game is already added.").Wait();
            } else
            {
                data.db.addGame(game);
                DiscordUtil.reply(context, "Game added.").Wait();
            }
        }
    }

    class GameRemoveCommand : Command
    {
        private SocketCommandContext context;
        private string game;

        public GameRemoveCommand(SocketCommandContext context, string game)
        {
            this.context = context;
            this.game = game;
        }

        public override void execute(EventHandler data)
        {
            if (data.db.gameExists(game))
            {
                data.db.removeGame(game);
                DiscordUtil.reply(context, "Game removed.").Wait();
            }
            else
            {
                DiscordUtil.replyError(context, "Game does not exist.").Wait();
            }
        }
    }

    class GameBuyCommand : Command
    {
        private SocketCommandContext context;
        private string game;

        public GameBuyCommand(SocketCommandContext context, string game)
        {
            this.context = context;
            this.game = game;
        }

        public override void execute(EventHandler data)
        {
            if(!data.db.gameExists(game))
            {
                DiscordUtil.replyError(context, "Game does not exist.").Wait();
                return;
            }
            SocketUser user = context.User;
            if(!data.db.playerExists(user.Id))
            {
                data.db.addPlayer(user.Id, user.Username);
                data.db.buyGame(user.Id, game);
                DiscordUtil.reply(context, "Game added to your library.").Wait();
                return;
            }
            if(data.db.hasGame(user.Id, game))
            {
                DiscordUtil.replyError(context, "You already own that game.").Wait();
            } else
            {
                data.db.buyGame(user.Id, game);
                DiscordUtil.reply(context, "Game added to your library.").Wait();
            }
        }
    }

    class GameGiftCommand : Command
    {
        private SocketCommandContext context;
        private SocketUser user;
        private string game;

        public GameGiftCommand(SocketCommandContext context, SocketUser user, string game)
        {
            this.context = context;
            this.user = user;
            this.game = game;
        }

        public override void execute(EventHandler data)
        {
            if (!data.db.gameExists(game))
            {
                DiscordUtil.replyError(context, "Game does not exist.").Wait();
                return;
            }
            if (!data.db.playerExists(user.Id))
            {
                data.db.addPlayer(user.Id, user.Username);
                data.db.buyGame(user.Id, game);
                DiscordUtil.reply(context, "Game added to " + user.Username + "'s library.").Wait();
                return;
            }
            if (data.db.hasGame(user.Id, game))
            {
                DiscordUtil.replyError(context, user.Username + " already owns that game.").Wait();
            }
            else
            {
                data.db.buyGame(user.Id, game);
                DiscordUtil.reply(context, "Game added to " + user.Username + "'s library.").Wait();
            }
        }
    }

    class GameSellCommand : Command
    {
        private SocketCommandContext context;
        private string game;

        public GameSellCommand(SocketCommandContext context, string game)
        {
            this.context = context;
            this.game = game;
        }

        public override void execute(EventHandler data)
        {
            SocketUser user = context.User;
            if(!data.db.playerExists(user.Id))
            {
                DiscordUtil.replyError(context, "You do not own any games.").Wait();
                return;
            }
            if(data.db.hasGame(user.Id, game))
            {
                data.db.sellGame(user.Id, game);
                DiscordUtil.reply(context, "Game removed from your library.").Wait();
            } else
            {
                DiscordUtil.replyError(context, "You do not own that game.").Wait();
            }
        }
    }
    #endregion
}
