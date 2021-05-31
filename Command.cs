using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine($"Status set to: {getActivity(type)}{status}");
        }

        private string getActivity(int i)
        {
            switch(i)
            {
                case 0:
                    return "Playing "; // ActivityType.Playing;
                //case 1:
                //    return "Streaming "; // ActivityType.Streaming;
                case 2:
                    return "Listening "; // ActivityType.Listening;
                case 3:
                    return "Watching "; // ActivityType.Watching;
                case 4:
                    return ""; // ActivityType.CustomStatus;
                default:
                    return "Playing "; // ActivityType.Playing;
            }
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


            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = DiscordUtil.italic("Help");
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
            string srng = $"~rng [{sx}] [{sn}] {sarrow} Calculate the probability of an occurrence, with an absolute chance of {sx}, to happen at least once in {sn} attempts.\n";
            string schance = $"~chance [{sx}] [{starget}] {sarrow} Calculate the number of attempts necessary for an occurrence, with an absolute chance of {sx}, to reach the target statistical chance {starget}.\n";
            string calc = $"[{exp}] {sarrow} Calculate the value of the expression.\n";
            math.Name = "Math";
            math.Value = srng + schance + calc;
            math.IsInline = false;
            embed.AddField(math);
            
            DiscordUtil.reply(context, embed: embed.Build()).Wait();
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
            string tm = message != null && message.Trim() != "" ? DiscordUtil.bold("Beep Beep: ") + DiscordUtil.highlight(message.Replace("*", "").Replace("_", "").Replace("`", "").Replace("|", "")) : DiscordUtil.bold("Beep Beep.");
            Task.Run(() => {
                TimeSpan time = new TimeSpan(0, minutes, 0);
                Thread.Sleep(time);
                data.queue.Add(new CommandEvent(new ReplyCommand(context, tm)));
            });
            context.Message.AddReactionAsync(new Emoji(EmojiUnicode.TIMER));
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
            SocketUser user = context.User;
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
        }
    }
    #endregion
}
