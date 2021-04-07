﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [Command("source")]
        [Alias("github")]
        [Summary("Returns the link to my public repository.")]
        public async Task SourceCommand()
        {
            queue.Add(new CommandEvent(new SourceCommand(Context)));
            await Task.CompletedTask;
        }

        [Command("roll")]
        [Summary("Randomly picks a number between 1 and a specified number inclusively, N (default: 1) times.")]
        public async Task RollCommand([Summary("The upper bound of the generation range.")] int max, [Summary("The (optional) amount of numbers rolled.")]  int n = 1)
        {
            if(max < 2)
            {
                await DiscordUtil.sendError(Context, "Max has to be 2 or higher.");
                return;
            }
            if(n < 1)
            {
                await DiscordUtil.sendError(Context, "N has to be 1 or higher.");
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
                await DiscordUtil.sendError(Context, "Max cannot be lower than min.");
                return;
            }
            queue.Add(new CommandEvent(new RandCommand(Context, min, max)));
        }

        [Command("rng")]
        [Summary("Calculates the chance of an occurrence happening at least once given it's absolute chance and number of attempts.")]
        public async Task RngCommand([Summary("The absolute chance of the occurrence.")] float x, [Summary("The number of attempts.")] int n)
        {
            if(1 <= x || x <= 0)
            {
                await DiscordUtil.sendError(Context, "Chance of the occurence has to be between 0 and 1 (both exclusive).");
                return;
            }
            if(n < 1)
            {
                await DiscordUtil.sendError(Context, "The number of attempts has to be atleast 1.");
                return;
            }
            queue.Add(new CommandEvent(new RngCommand(Context, x, n)));
        }

        [Command("pick")]
        [Summary("Randomly chooses N unique numbers between 1 and X (inclusive).")]
        public async Task PickCommand([Summary("The amount of numbers to be chosen.")] int n, [Summary("The upper bound of the range from which numbers are chosen.")] int max)
        {
            if (max < 2)
            {
                await DiscordUtil.sendError(Context, "Max should be 2 or higher.");
                return;
            }
            if (max < n)
            {
                await DiscordUtil.sendError(Context, "Max cannot be lower than the amount of numbers chosen.");
                return;
            }
            queue.Add(new CommandEvent(new PickCommand(Context, n, max)));
        }

        [Command("chance")]
        [Summary("Calculates the number of attempts after which an occurrence with a given absolute chance reaches the given statistical chance threshold.")]
        public async Task ChanceCommand([Summary("The absolute chance of the occurrence.")] float x, [Summary("The chance threshold to be reached.")] float t)
        {
            if (1 <= x || x <= 0)
            {
                await DiscordUtil.sendError(Context, "Chance of the occurence has to be between 0 and 1 (both exclusive).");
                return;
            }
            if (1 <= t || t <= 0)
            {
                await DiscordUtil.sendError(Context, "Chance threshold has to be between 0 and 1 (both exclusive).");
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
    }
}