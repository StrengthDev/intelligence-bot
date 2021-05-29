using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    class Program
    {
        static ConcurrentQueue<Event> innerQueue;
        static BlockingCollection<Event> eventQueue;

        static void Main(string[] args)
        {
            Dictionary<string, string> config = IO.readINI(@".\config.ini");

            innerQueue = new ConcurrentQueue<Event>();
            eventQueue = new BlockingCollection<Event>(innerQueue);

            Bot bot = new Bot(eventQueue, config);
            Task running = Task.Run(() => bot.run());

            RemoteCommandParser.SetQueue(eventQueue);
            RemoteGameCommandParser.SetQueue(eventQueue);

            LocalCommandReader lcr = new LocalCommandReader(eventQueue);
            Task.Run(() => lcr.run());

            running.Wait();
        }
    }
}
