﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Globalization;

namespace intelligence_bot
{
    //[Group("prefix")]
    public class RemoteCommandParser : ModuleBase<SocketCommandContext>
    {
        private static BlockingCollection<Event> queue;

        internal static void SetQueue(BlockingCollection<Event> queue)
        {
            RemoteCommandParser.queue = queue;
        }

        [Command("roll")]
        [Summary("Randomly picks a number between 1 and a specified number inclusively, N (default: 1) times.")]
        public async Task RollCommand([Summary("The upper bound of the generation range.")] int max, [Summary("The (optional) amount of numbers rolled.")]  int n = 1)
        {
            if(max < 2)
            {
                await DiscordUtil.replyError(Context, "Max has to be 2 or higher.");
                return;
            }
            if(n < 1)
            {
                await DiscordUtil.replyError(Context, "N has to be 1 or higher.");
                return;
            }
            queue.Add(new CommandEvent(new RollCommand(Context, max, n)));
        }

        [Command("rand")]
        [Summary("Generates a random number between X (inclusive) and Y (exclusive).")]
        public async Task RandCommand([Summary("The lower bound of the generated number.")] int min, [Summary("The upper bound of the generated number.")] int max)
        {
            if(max < min)
            {
                await DiscordUtil.replyError(Context, "Max cannot be lower than min.");
                return;
            }
            queue.Add(new CommandEvent(new RandCommand(Context, min, max)));
        }

        [Command("pick")]
        [Summary("Randomly chooses N unique numbers between 1 and X (inclusive).")]
        public async Task PickCommand([Summary("The amount of numbers to be chosen.")] int n, [Summary("The upper bound of the range from which numbers are chosen.")] int max)
        {
            if (max < 2)
            {
                await DiscordUtil.replyError(Context, "Max should be 2 or higher.");
                return;
            }
            if (max < n)
            {
                await DiscordUtil.replyError(Context, "Max cannot be lower than the amount of numbers chosen.");
                return;
            }
            queue.Add(new CommandEvent(new PickCommand(Context, n, max)));
        }

        [Command("rng")]
        [Summary("Calculates the chance of an occurrence happening at least once given it's absolute chance and number of attempts.")]
        public async Task RngCommand([Summary("The absolute chance of the occurrence.")] float x, [Summary("The number of attempts.")] int n)
        {
            if(1 <= x || x <= 0)
            {
                await DiscordUtil.replyError(Context, "Chance of the occurence has to be between 0 and 1 (both exclusive).");
                return;
            }
            if(n < 1)
            {
                await DiscordUtil.replyError(Context, "The number of attempts has to be atleast 1.");
                return;
            }
            queue.Add(new CommandEvent(new RngCommand(Context, x, n)));
        }

        [Command("chance")]
        [Summary("Calculates the number of attempts after which an occurrence with a given absolute chance reaches the given statistical chance threshold.")]
        public async Task ChanceCommand([Summary("The absolute chance of the occurrence.")] float x, [Summary("The chance threshold to be reached.")] float t)
        {
            if (1 <= x || x <= 0)
            {
                await DiscordUtil.replyError(Context, "Chance of the occurence has to be between 0 and 1 (both exclusive).");
                return;
            }
            if (1 <= t || t <= 0)
            {
                await DiscordUtil.replyError(Context, "Chance threshold has to be between 0 and 1 (both exclusive).");
                return;
            }
            queue.Add(new CommandEvent(new ChanceCommand(Context, x, t)));
        }

        [Command("help")]
        [Summary("Describes every command.")]
        public async Task HelpCommand()
        {
            queue.Add(new CommandEvent(new HelpCommand(Context)));
            await Task.CompletedTask;
        }

        [Command("source")]
        [Alias("github")]
        [Summary("Returns the link to my public repository.")]
        public async Task SourceCommand()
        {
            queue.Add(new CommandEvent(new SourceCommand(Context)));
            await Task.CompletedTask;
        }

        [Command("profile")]
        [Summary("Lists information about the specified user, or the one who used the command if none specified.")]
        [Alias("user", "whois", "who")]
        public async Task ProfileCommand([Summary("The (optinal) user.")] SocketUser user = null)
        {
            SocketUser target = user ?? Context.Message.Author;
            queue.Add(new CommandEvent(new ProfileCommand(Context, target)));
            await Task.CompletedTask;
        }

        [Command("timer")]
        [Summary("Sets a timer, after which the user is pinged.")]
        public async Task TimerCommand([Summary("Minutes to wait.")] int minutes, [Remainder][Summary("Message of the timer.")] string message = null)
        {
            if (minutes < 1)
            {
                await DiscordUtil.replyError(Context, "Invalid number.");
                return;
            }
            if (minutes > 1440)
            {
                await DiscordUtil.replyError(Context, "Time limited to at most, 24 hours.");
                return;
            }
            queue.Add(new CommandEvent(new TimerCommand(Context, minutes, message)));
        }

        [Command("remind")]
        [Summary("Pings the user at the point in time time provided.")]
        public async Task RemindCommand([Remainder][Summary("Arguments.")] string message)
        {
            int space0 = message.IndexOf(' ');
            int space1 = message.IndexOf(' ', space0 + 1);
            string dateString;
            string userMessage;
            if(space1 == -1)
            {
                dateString = message;
                userMessage = "";
            } else
            {
                dateString = message.Substring(0, space1);
                userMessage = message.Substring(space1 + 1);
            }
            DateTime date;
            try
            {
                DateTime.TryParseExact(dateString, "g", CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out date);
            } catch (ArgumentException)
            {
                await DiscordUtil.replyError(Context, "Invalid date./nDate format: dd/mm/yyyy hh:mm");
                return;
            }
            queue.Add(new CommandEvent(new RemindCommand(Context, date, userMessage)));
        }

        [Command("now")]
        [Summary("Displays current time.")]
        public async Task NowCommand()
        {
            queue.Add(new CommandEvent(new ReplyCommand(Context, DateTime.Now.ToString("R"))));
            await Task.CompletedTask;
        }

        [Command("ask")]
        [Summary("Provides an answer to the question.")]
        public async Task AskCommand([Remainder][Summary("The question.")] string question)
        {
            queue.Add(new CommandEvent(new AskCommand(Context, question)));
            await Task.CompletedTask;
        }

        [Command("powerlevel")]
        [Summary("Displays the power level of the calling user.")]
        public async Task PowerLevelCommand()
        {
            queue.Add(new CommandEvent(new PowerLevelCommand(Context)));
            await Task.CompletedTask;
        }

        [Command("emoji")]
        [Summary("Lists information about the emoji.")]
        public async Task EmojiCommand([Remainder][Summary("Emoji to be analyzed.")] string emoji)
        {
            string e = emoji.Trim();
            if (e.Length != 2)
            {
                await DiscordUtil.replyError(Context, $"Not an Emote. ({e.Length})");
                return;
            }
            queue.Add(new CommandEvent(new EmojiCommand(Context, e)));
        }

        [Command("test")]
        [Summary(".")]
        public async Task GenerateCommand([Remainder][Summary(".")] string generator)
        {
            queue.Add(new CommandEvent(new GenerateCommand(Context, generator)));
            await Task.CompletedTask;
        }
    }

    [Group("text")]
    public class RemoteTextCommandParser : ModuleBase<SocketCommandContext>
    {
        private static BlockingCollection<Event> queue;

        internal static void SetQueue(BlockingCollection<Event> queue)
        {
            RemoteTextCommandParser.queue = queue;
        }

        [Group("regex")]
        public class RemoteRegexCommandParser : ModuleBase<SocketCommandContext>
        {
            [Command("replace")]
            [Summary("Replaces every instance of text within the previous message, matching the regex, with the replacement.")]
            public async Task RegexReplaceCommand([Remainder][Summary("Arguments for the command.")] string args)
            {
                queue.Add(new CommandEvent(new RegexReplaceCommand(Context, args)));
                await Task.CompletedTask;
            }
        }

    }

    [Group("games")]
    public class RemoteGameCommandParser : ModuleBase<SocketCommandContext>
    {
        private static BlockingCollection<Event> queue;

        internal static void SetQueue(BlockingCollection<Event> queue)
        {
            RemoteGameCommandParser.queue = queue;
        }

        [Command]
        [Summary("Lists games of the specified user, or the system if none specified.")]
        public async Task DefaultCommand([Summary("The (optinal) user whose games are to be listed.")] SocketUser user = null)
        {
            if(user == null)
            {
                queue.Add(new CommandEvent(new GameSListCommand(Context)));
            } else
            {
                queue.Add(new CommandEvent(new GamePListCommand(Context, user)));
            }
            await Task.CompletedTask;
        }

        [Command("intersect")]
        [Summary("Lists games of the specified user, or the system if none specified.")]
        public async Task IntersectCommand([Remainder][Summary(".")] string users)
        {
            List<SocketUser> sus = new List<SocketUser>();
            foreach(string user in users.Split(' '))
            {
                TypeReaderResult res = new UserTypeReader<SocketUser>().ReadAsync(Context, user, null).Result;
                if(res.IsSuccess)
                {
                    SocketUser su = (SocketUser)res.BestMatch;
                    sus.Add(su);
                } else
                {
                    await DiscordUtil.replyError(Context, "Could not parse: " + user);
                    return;
                }
            }
            queue.Add(new CommandEvent(new GameIntersectionCommand(Context, sus.ToArray())));
            await Task.CompletedTask;
        }

        [Command("add")]
        [Summary(".")]
        public async Task AddCommand([Remainder][Summary(".")] string game)
        {
            queue.Add(new CommandEvent(new GameAddCommand(Context, game)));
            await Task.CompletedTask;
        }

        [Command("remove")]
        [Summary(".")]
        public async Task RemoveCommand([Remainder][Summary(".")] string game)
        {
            queue.Add(new CommandEvent(new GameRemoveCommand(Context, game)));
            await Task.CompletedTask;
        }

        [Command("buy")]
        [Summary(".")]
        public async Task BuyCommand([Remainder][Summary(".")] string game)
        {
            queue.Add(new CommandEvent(new GameBuyCommand(Context, game)));
            await Task.CompletedTask;
        }

        [Command("gift")]
        [Summary(".")]
        public async Task GiftCommand([Summary(".")] SocketUser user, [Remainder][Summary(".")] string game)
        {
            queue.Add(new CommandEvent(new GameGiftCommand(Context, user, game)));
            await Task.CompletedTask;
        }

        [Command("sell")]
        [Summary(".")]
        public async Task SellCommand([Remainder][Summary(".")] string game)
        {
            queue.Add(new CommandEvent(new GameSellCommand(Context, game)));
            await Task.CompletedTask;
        }
    }
}
