using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    class LocalCommandReader
    {
        private BlockingCollection<Event> queue;

        public LocalCommandReader(BlockingCollection<Event> queue)
        {
            this.queue = queue;
        }

        public void run()
        {
            string command;
            while ((command = Console.ReadLine()) != "exit")
            {
                queue.Add(new CommandEvent(LocalCommandParser.parse(command)));
            }
            Console.WriteLine("Exiting..");
            queue.CompleteAdding();
        }
    }
}
