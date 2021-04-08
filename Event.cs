using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace intelligence_bot
{
    enum EventType
    {
        NONE,
        COMMAND,
        MESSAGE,
        REACTION
    }

    class Event
    {
        public EventType type { get; private set; }

        protected Event(EventType type)
        {
            this.type = type;
        }

        static public Event emptyEvent = new Event(EventType.NONE);
    }

    class CommandEvent : Event
    {
        public Command com { get; private set; }

        public CommandEvent(Command com) : base(EventType.COMMAND)
        {
            this.com = com;
        }
    }

    class MessageEvent : Event
    {
        public SocketCommandContext context { get; private set; }

        public MessageEvent(SocketCommandContext context) : base(EventType.MESSAGE)
        {
            this.context = context;
        }
    }
}