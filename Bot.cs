using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace intelligence_bot
{
    class Bot
    {
        private BlockingCollection<Event> queue;
        private Dictionary<string, string> config;
        private DiscordSocketClient socket;
        private EventHandler handler;

        public Bot(BlockingCollection<Event> queue, Dictionary<string, string> config)
        {
            this.queue = queue;
            this.config = config;
            socket = new DiscordSocketClient();
            bool mentionCmds = false;
            if (config.ContainsKey("mentionCommands"))
            {
                string bt;
                config.TryGetValue("mentionCommands", out bt);
                mentionCmds = bool.Parse(bt);
            }
            handler = new EventHandler(socket, mentionCmds);
        }

        public async Task run()
        {
            await init();
            try
            {
                while(true)
                {
                    handler.handleEvent(queue.Take());
                }
            } catch (InvalidOperationException) { }
            await shutdown();
        }

        private async Task init()
        {

            socket.Log += Log;
            string token;
            if(!config.TryGetValue("token", out token))
            {
                //TODO exception
            }
            await socket.LoginAsync(Discord.TokenType.Bot, token);
            await socket.StartAsync();
            socket.Ready += () =>
            {
                Console.WriteLine("Bot synced");
                return Task.CompletedTask;
            };
            if(config.ContainsKey("status"))
            {
                string status;
                int type = 0;
                if(config.ContainsKey("statusType"))
                {
                    string stype;
                    config.TryGetValue("statusType", out stype);
                    type = int.Parse(stype);
                }
                config.TryGetValue("status", out status);
                queue.Add(new CommandEvent(new SetStatusCommand(status, type)));
            }
        }

        private async Task shutdown()
        {
            await socket.LogoutAsync();
            await socket.StopAsync();
        }

        static private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
