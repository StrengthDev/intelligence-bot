using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace intelligence_bot
{
    class EventHandler
    {
        public DiscordSocketClient socket { get; }
        public SocketGuild[] servers { get; set; }
        public SocketTextChannel[] channels { get; set; }

        public int currentServer { get; set; }
        public int currentChannel { get; set; }

        public EventHandler(DiscordSocketClient socket)
        {
            this.socket = socket;
            servers = null;
            channels = null;
            currentServer = -1;
            currentChannel = -1;
        }

        public void handleEvent(Event e)
        {
            switch (e.type)
            {
                case EventType.COMMAND:
                    ((CommandEvent)e).com.execute(this);
                    break;
            }
        }
    }
}
