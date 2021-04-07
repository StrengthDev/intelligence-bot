using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    class SourceCommand : Command
    {
        private SocketCommandContext context;

        public SourceCommand(SocketCommandContext context)
        {
            this.context = context;
        }

        public override void execute(EventHandler data)
        {
            context.Channel.SendMessageAsync("https://github.com/StrengthDev/intelligence-bot");
        }
    }

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
            context.Channel.SendMessageAsync(DiscordUtil.code(message)).Wait();
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
            context.Channel.SendMessageAsync(DiscordUtil.bold(data.rng.Next(min, max))).Wait();
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
            context.Channel.SendMessageAsync($"{DiscordUtil.bold(cs)} chance for an occurrence, with an absolute chance of {x * 100.0f}%, to happen at least once after {n} attempts.").Wait();
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
            context.Channel.SendMessageAsync(DiscordUtil.code(message)).Wait();
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
            string message = $"Within {DiscordUtil.bold(attempts)} attempts, an occurence with {x * 100.0f}% absolute chance, reaches a statistical chance of {cs} to happen.";
            context.Channel.SendMessageAsync(message).Wait();
        }
    }
}
