using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    static class LocalCommandParser
    {
        public static Command parse(string com)
        {
            int index = com.IndexOf(' ');
            string fn;
            string args = string.Empty;
            if(index != -1)
            {
                fn = com.Substring(0, index);
                args = com.Substring(index + 1, com.Length - index - 1);
            } else
            {
                fn = com;
            }
            switch(fn)
            {
                case "listservers":
                    return new LServersCommand();
                case "listchannels":
                    return new LChannelsCommand();
                case "selectserver":
                    return new SelectServerCommand(int.Parse(args));
                case "selectchannel":
                    return new SelectChannelCommand(int.Parse(args));
                case "current":
                    return new CurrentCommand();
                case "say":
                    return new SayCommand(args);
            }
            Console.WriteLine("Unknown command.");
            return null;
        }
    }
}
