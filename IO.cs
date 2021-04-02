using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    class IO
    {
        public static Dictionary<string, string> readINI(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if(line.Length < 4)
                {
                    continue;
                }
                if(line.StartsWith("["))
                {
                    continue;
                }
                int index = line.IndexOf('=');
                config.Add(line.Substring(0, index), line.Substring(index + 1, line.Length - index - 1));
            }
            return config;
        }
    }
}
