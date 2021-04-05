using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
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
}
