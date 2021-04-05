using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace intelligence_bot
{
    class EventHandler
    {
        public DiscordSocketClient socket { get; private set; }
        public CommandService cmdService { get; private set; }
        public SocketGuild[] servers { get; set; }
        public SocketTextChannel[] channels { get; set; }

        public int currentServer { get; set; }
        public int currentChannel { get; set; }
        private char prefix = '~';

        public EventHandler(DiscordSocketClient socket, bool mentionCmds)
        {
            this.socket = socket;
            servers = null;
            channels = null;
            currentServer = -1;
            currentChannel = -1;
            //TODO configuration
            cmdService = new CommandService();
            if(mentionCmds)
            {
                socket.MessageReceived += remoteCommandWithMention;
            } else
            {
                socket.MessageReceived += remoteCommand;
            }
            cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), null).Wait();
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

        private async Task remoteCommand(SocketMessage messageData)
        {
            SocketUserMessage message = messageData as SocketUserMessage;
            if(message == null)
            {
                return;
            }
            int index = 0;
            if(!message.HasCharPrefix(prefix, ref index))
            {
                return;
            }
            SocketCommandContext context = new SocketCommandContext(socket, message);
            await cmdService.ExecuteAsync(context, index, null);
        }

        private async Task remoteCommandWithMention(SocketMessage messageData)
        {
            SocketUserMessage message = messageData as SocketUserMessage;
            if (message == null)
            {
                return;
            }
            int index = 0;
            if (!(message.HasCharPrefix(prefix, ref index) || message.HasMentionPrefix(socket.CurrentUser, ref index)))
            {
                return;
            }
            SocketCommandContext context = new SocketCommandContext(socket, message);
            await cmdService.ExecuteAsync(context, index, null);
        }
    }
}
