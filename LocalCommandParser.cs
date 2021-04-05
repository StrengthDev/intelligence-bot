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
            string arg = string.Empty;
            string[] args;
            if(index != -1)
            {
                fn = com.Substring(0, index);
                arg = com.Substring(index + 1, com.Length - index - 1);
            } else
            {
                fn = com;
            }
            switch(fn)
            {
                case "ls":
                case "listservers":
                    return new LServersCommand();
                case "lc":
                case "listchannels":
                    return new LChannelsCommand();
                case "ss":
                case "selectserver":
                    return new SelectServerCommand(int.Parse(arg));
                case "sc":
                case "selectchannel":
                    return new SelectChannelCommand(int.Parse(arg));
                case "current":
                    return new CurrentCommand();
                case "say":
                    return new SayCommand(arg);
                case "setstatus":
                    int ni = arg.IndexOf(' ');
                    int t = int.Parse(arg.Substring(0, ni));
                    string s = arg.Substring(ni + 1, arg.Length - ni - 1);
                    return new SetStatusCommand(s, t);
            }
            Console.WriteLine("Unknown command.");
            return null;
        }

        static private string[] splitArgs(string arg)
        {
            return null;
        }
    }
}
