﻿using System;
using System.Collections.Concurrent;
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
        public BlockingCollection<Event> queue { get; private set; }
        public DiscordSocketClient socket { get; private set; }
        public CommandService cmdService { get; private set; }
        public SocketGuild[] servers { get; set; }
        public SocketTextChannel[] channels { get; set; }

        public int currentServer { get; set; }
        public int currentChannel { get; set; }

        public GameDatabase db { get; private set; }

        public Dictionary<string, string> config;
        public Tuple<uint, string>[] answers;
        public string plf0 = "";
        public string plf1 = "";
        public string plf2 = "";
        public uint plf_length = 1;
        public Random rng;
        private char prefix = '~';

        //Generator gen; TODO: uncomment when implemented

        public EventHandler(BlockingCollection<Event> queue, DiscordSocketClient socket, Dictionary<string, string> config)
        {
            this.queue = queue;
            this.socket = socket;
            servers = null;
            channels = null;
            currentServer = -1;
            currentChannel = -1;
            cmdService = new CommandService();
            this.config = config;
            answers = initAnswers();
            initPowerLevelFormat();
            string temps;
            bool mentionCmds = false;
            if (config.ContainsKey(ConfigKeyword.MENTION_COMMANDS))
            {
                config.TryGetValue(ConfigKeyword.MENTION_COMMANDS, out temps);
                mentionCmds = bool.Parse(temps);
            }
            if (mentionCmds)
            {
                socket.MessageReceived += remoteCommandWithMention;
            } else
            {
                socket.MessageReceived += remoteCommand;
            }
            if (config.ContainsKey(ConfigKeyword.GDB_LOC))
            {
                config.TryGetValue(ConfigKeyword.GDB_LOC, out temps);
            } else
            {
                temps = "DEFAULT_GAME_DATABASE.xml";
            }
            db = new GameDatabase(temps);
            db.load();
            rng = new Random();
            cmdService.AddModulesAsync(Assembly.GetEntryAssembly(), null).Wait();

            //gen = new Generator(@"C:\dev\image-generator-trainer\model"); TODO: uncomment when implemented
        }

        public void shutdown()
        {
            db.save();
        }

        public void handleEvent(Event e)
        {
            switch (e.type)
            {
                case EventType.COMMAND:
                    ((CommandEvent)e).com.execute(this);
                    break;
                case EventType.MESSAGE:
                    parseMessage(((MessageEvent)e).context);
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
            SocketCommandContext context = new SocketCommandContext(socket, message);
            if(!message.HasCharPrefix(prefix, ref index))
            {
                queue.Add(new MessageEvent(context));
                return;
            }
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
            SocketCommandContext context = new SocketCommandContext(socket, message);
            if (!(message.HasCharPrefix(prefix, ref index) || message.HasMentionPrefix(socket.CurrentUser, ref index)))
            {
                queue.Add(new MessageEvent(context));
                return;
            }
            await cmdService.ExecuteAsync(context, index, null);
        }

        private void parseMessage(SocketCommandContext context)
        {
            try
            {
                double result = parseExpression(context.Message.Content.ToLower().Replace(" ", ""));
                DiscordUtil.reply(context, DiscordUtil.bold(string.Format("{0,0:0.###}", result))).Wait();
            } catch (Exception) { } finally
            {
                if(context.Message.Content.Trim() == (EmojiUnicode.POINT_RIGHT + ' ' + EmojiUnicode.POINT_RIGHT))
                {
                    DiscordUtil.sendMessage(context, ":point_left: :point_left:").Wait();
                }
            }
        }

        private Tuple<uint, string>[] initAnswers()
        {
            try
            {
                string src;
                config.TryGetValue(ConfigKeyword.ANSWERS, out src);
                string[] unformated = src.Split(ConfigKeyword.ANSWER_SEPERATOR);
                Tuple<uint, string>[] res = new Tuple<uint, string>[unformated.Length];
                int i = 0;
                uint total = 0;
                foreach(string s in unformated)
                {
                    int index = s.IndexOf(ConfigKeyword.VALUE_SEPERATOR);
                    string x = s.Substring(0, index);
                    string b = s.Substring(index + 1);
                    total += uint.Parse(x);
                    res[i] = new Tuple<uint, string>(total, b);
                    i++;
                }
                return res;
            } catch (Exception)
            {
                return null;
            }
        }

        private void initPowerLevelFormat()
        {
            try
            {
                string src;
                config.TryGetValue(ConfigKeyword.PL_FORMAT, out src);
                string[] pieces = src.Split(ConfigKeyword.FORMAT_SEPERATOR);
                plf0 = pieces[0];
                plf1 = pieces[1];
                plf2 = pieces[2];
                plf_length = uint.Parse(pieces[3]);
            }
            catch (Exception)
            { }
        }

        private const char PLUS = '+';
        private const char MINUS = '-';
        private const char MULT = '*';
        private const char DIV = '/';
        private const char MOD = '%';
        private const char EXP = '^';

        private const char OPEN = '(';
        private const char CLOSE = ')';

        private const string PI = "pi";
        private const string E = "e";

        private const string COS = "cos";
        private const string SIN = "sin";
        private const string TAN = "tan";
        private const string ACOS = "acos";
        private const string ASIN = "asin";
        private const string ATAN = "atan";
        private const string LOG = "log";
        private const string CEIL = "ceil";
        private const string FLOOR = "floor";

        private const string ROLL = "roll";
        private const string RAND = "rand";

        private double parseExpression(string expression)
        {
            string trimmed = expression;
            int scopel;
            while((scopel = scopeLength(trimmed)) == trimmed.Length - 2)
            {
                trimmed = trimmed.Substring(1, trimmed.Length - 2);
            }

            int index = -1;
            if (trimmed.StartsWith(COS) && scopel == (trimmed.Length - COS.Length - 2))
            {
                index = COS.Length + 1;
                return Math.Cos(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(SIN) && scopel == (trimmed.Length - SIN.Length - 2))
            {
                index = SIN.Length + 1;
                return Math.Sin(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(TAN) && scopel == (trimmed.Length - TAN.Length - 2))
            {
                index = TAN.Length + 1;
                return Math.Tan(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(ACOS) && scopel == (trimmed.Length - ACOS.Length - 2))
            {
                index = ACOS.Length + 1;
                return Math.Acos(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(ASIN) && scopel == (trimmed.Length - ASIN.Length - 2))
            {
                index = ASIN.Length + 1;
                return Math.Asin(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(ATAN) && scopel == (trimmed.Length - ATAN.Length - 2))
            {
                index = ATAN.Length + 1;
                return Math.Cos(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(LOG) && scopel == (trimmed.Length - LOG.Length - 2))
            {
                index = LOG.Length + 1;
                return Math.Log(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(CEIL) && scopel == (trimmed.Length - CEIL.Length - 2))
            {
                index = CEIL.Length + 1;
                return Math.Ceiling(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(FLOOR) && scopel == (trimmed.Length - FLOOR.Length - 2))
            {
                index = FLOOR.Length + 1;
                return Math.Floor(parseExpression(trimmed.Substring(index, trimmed.Length - index - 1)));
            }
            if (trimmed.StartsWith(ROLL) && scopel == (trimmed.Length - ROLL.Length - 2))
            {
                index = ROLL.Length + 1;
                return Math.Floor(rng.NextDouble() * parseExpression(trimmed.Substring(index, trimmed.Length - index - 1))) + 1.0d;
            }
            if (trimmed.StartsWith(RAND) && scopel == (trimmed.Length - RAND.Length - 2))
            {
                index = RAND.Length + 1;
                return rng.NextDouble() * parseExpression(trimmed.Substring(index, trimmed.Length - index - 1));
            }

            int depth = 0;
            int tier = int.MaxValue;
            char op = '\0';
            int pindex = 0;
            foreach(char c in trimmed)
            {
                index++;
                if(depth < 0)
                {
                    throw new ArgumentException();
                }
                if(c == OPEN)
                {
                    depth++;
                    continue;
                } else if(c == CLOSE)
                {
                    depth--;
                    continue;
                }
                if(depth == 0)
                {
                    if(c == PLUS)
                    {
                        op = PLUS;
                        pindex = index;
                        break;
                    } else if(c == MINUS)
                    {
                        op = MINUS;
                        pindex = index;
                        break;
                    }
                    switch(c)
                    {
                        case MULT:
                            op = MULT;
                            tier = 1;
                            pindex = index;
                            break;
                        case DIV:
                            op = DIV;
                            tier = 1;
                            pindex = index;
                            break;
                        case MOD:
                            op = MOD;
                            tier = 1;
                            pindex = index;
                            break;
                        case EXP:
                            if(1 < tier)
                            {
                                op = EXP;
                                tier = 2;
                                pindex = index;
                            }
                            break;
                    }
                }
            }
            if (depth != 0)
            {
                throw new ArgumentException();
            }
            string left = trimmed.Substring(0, pindex);
            string right = trimmed.Substring(pindex + 1, trimmed.Length - pindex - 1);
            switch (op)
            {
                case PLUS:
                    return parseExpression(left) + parseExpression(right);
                case MINUS:
                    left = left.Length == 0 ? "0" : left;
                    return parseExpression(left) - parseExpression(right);
                case MULT:
                    return parseExpression(left) * parseExpression(right);
                case DIV:
                    return parseExpression(left) / parseExpression(right);
                case MOD:
                    return parseExpression(left) % parseExpression(right);
                case EXP:
                    return Math.Pow(parseExpression(left), parseExpression(right));
            }
            switch(trimmed)
            {
                case PI:
                    return Math.PI;
                case E:
                    return Math.E;
                default:
                    return double.Parse(trimmed);
            }
        }

        private static int scopeLength(string expression)
        {
            int index = -1;
            int open = -1;
            int depth = 0;
            foreach(char c in expression)
            {
                index++;
                if(c == OPEN)
                {
                    depth++;
                    if(open == -1)
                    {
                        open = index;
                    }
                } else if(c == CLOSE)
                {
                    depth--;
                }
                if (open != -1 && depth == 0)
                {
                    break;
                }
            }
            return index - open - 1;
        }
    }
}
