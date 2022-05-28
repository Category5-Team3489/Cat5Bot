namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("schedule"), Aliases("s"), Description("Links a user's name to their account.")]
public class EventsModule : BaseCommandModule
{
    // TODO event editing?

    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task ScheduleHelp(CommandContext ctx)
    {
        Log.Command("schedule help", ctx.User.ToString());

        await ctx.RespondAsync(Constants.Messages.ScheduleHelp);
    }

    [Command("help"), Description("Links your full name to your attendance record.")]
    public async Task ScheduleHelpExplicit(CommandContext ctx)
    {
        await ScheduleHelp(ctx);
    }

    [Command("list"), Description("Links your full name to your attendance record.")]
    public async Task ScheduleList(CommandContext ctx)
    {
        Log.Command("schedule list", ctx.User.ToString());

        StringBuilder list = new("Future events:\n");

        lock (Cat5BotDB.I.Lock)
        {
            int index = 1;
            foreach (ScheduledEvent scheduledEvent in Cat5BotDB.I.Events.GetFuture().Take(10))
            {
                list.Append($"{index}. \"{scheduledEvent.name}\": {scheduledEvent.CloneAsLocal().Summarize()}\n");
                index++;
            }
        }

        await ctx.RespondAsync(list.ToString());
    }

    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task Schedule(CommandContext ctx, params string[] args)
    {
        Log.Command("schedule", ctx.User.ToString());

        // checks
        if (args.Length < 5)
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("all 5 arguments must be satisfied"));
            return;
        }

        // eventDate 0
        (bool eventDateValid, DateTime eventDate) = await GetEventDate(ctx, args[0]);
        if (!eventDateValid) return;

        // eventTime 1
        (bool eventTimeValid, TimeSpan eventTime) = await GetEventTime(ctx, args[1]);
        if (!eventTimeValid) return;

        // eventLength 2
        (bool eventLengthValid, TimeSpan eventLength) = await GetEventLength(ctx, args[2]);
        if (!eventLengthValid) return;

        // eventType 3
        string eventType = args[3];

        // eventName 4..n
        string eventName = string.Join(' ', args[4..]);

        DateTime eventStart = eventDate.Add(eventTime).ToUniversalTime();
        ScheduledEvent scheduledEvent = new(eventName, eventType, eventStart, eventLength);

        ulong eventId;
        lock (Cat5BotDB.I.Lock)
        {
            eventId = Cat5BotDB.I.Events.Add(scheduledEvent);
        }

        // TODO add event overlap detection? ask if intentional?
        string scheduledEventText = $"Created \"{scheduledEvent.name}\": {scheduledEvent.Summarize()}";
        if (eventStart < DateTime.UtcNow)
        {
            scheduledEventText =
                $"{scheduledEventText}\n" +
                $"You created this event in the past, was that intentional?";
        }

        var interactivity = ctx.Client.GetInteractivity();
        var msg = await ctx.RespondAsync(
            $"{scheduledEventText}\n" +
            $"Hit the \"X\" to cancel, timeout in {Constants.ScheduledEventCancelTimeout}s"
        );
        var x = DiscordEmoji.FromName(ctx.Client, ":x:");
        await msg.CreateReactionAsync(x);
        bool cancelled = !(await interactivity.WaitForReactionAsync(xe => xe.Emoji == x && xe.Message == msg, ctx.User, TimeSpan.FromSeconds(Constants.ScheduledEventCancelTimeout))).TimedOut;
        if (cancelled)
        {
            lock (Cat5BotDB.I.Lock)
            {
                Cat5BotDB.I.Events.TryRemove(eventId, out _);
            }
            await msg.DeleteAsync();
        }
        else
        {
            await msg.DeleteOwnReactionAsync(x);
            await msg.ModifyAsync(scheduledEventText);
        }
    }

    private async Task<(bool, DateTime)> GetEventDate(CommandContext ctx, string arg)
    {
        async Task RespondInvalidNumbers()
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventDate> invalid values", Constants.Messages.ScheduleHelpEventDate()));
        }

        DateTime eventDate = DateTime.MinValue;
        string[] eventDateSplit = arg.Split('/');
        if (eventDateSplit.Length == 2)
        {
            if (!int.TryParse(eventDateSplit[0], out int month) || !int.TryParse(eventDateSplit[1], out int day))
            {
                await RespondInvalidNumbers();
                return (false, eventDate);
            }
            try
            {
                eventDate = new DateTime(DateTime.Now.Year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                await RespondInvalidNumbers();
                return (false, eventDate);
            }
        }
        else if (eventDateSplit.Length == 3)
        {
            if (!int.TryParse(eventDateSplit[0], out int month) || !int.TryParse(eventDateSplit[1], out int day) || !int.TryParse(eventDateSplit[2], out int year))
            {
                await RespondInvalidNumbers();
                return (false, eventDate);
            }
            try
            {
                eventDate = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                await RespondInvalidNumbers();
                return (false, eventDate);
            }
        }
        else
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventDate> invalid date syntax", Constants.Messages.ScheduleHelpEventDate()));
            return (false, eventDate);
        }
        return (true, eventDate);
    }

    private async Task<(bool, TimeSpan)> GetEventTime(CommandContext ctx, string arg)
    {
        async Task RespondInvalid()
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventTime> invalid time syntax", Constants.Messages.ScheduleHelpEventTime()));
        }

        TimeSpan eventTime = TimeSpan.FromSeconds(0);

        if (arg.Length < 3)
        {
            await RespondInvalid();
            return (false, eventTime);
        }

        string lastTwo = arg[(arg.Length-2)..];
        lastTwo = lastTwo.ToLower();

        if (lastTwo == "am") { }
        else if (lastTwo == "pm")
        {
            eventTime = eventTime.Add(new TimeSpan(12, 0, 0));
        }
        else
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventTime> must use am/pm notation", Constants.Messages.ScheduleHelpEventTime()));
            return (false, eventTime);
        }

        bool hasColon = arg.Contains(':');
        if (hasColon)
        {
            string[] eventTimeSplit = arg.Split(':');
            if (eventTimeSplit.Length != 2 || eventTimeSplit[1].Length < 3 || !int.TryParse(eventTimeSplit[0], out int hour) || !int.TryParse(eventTimeSplit[1][..(eventTimeSplit[1].Length-2)], out int min))
            {
                await RespondInvalid();
                return (false, eventTime);
            }
            eventTime = eventTime.Add(new TimeSpan(hour, min, 0));
        }
        else
        {
            if (!int.TryParse(arg[..(arg.Length-2)], out int hour))
            {
                await RespondInvalid();
                return (false, eventTime);
            }
            eventTime = eventTime.Add(new TimeSpan(hour, 0, 0));
        }
        return (true, eventTime);
    }

    private async Task<(bool, TimeSpan)> GetEventLength(CommandContext ctx, string arg)
    {
        async Task RespondInvalid()
        {
            await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventLength> invalid length syntax", Constants.Messages.ScheduleHelpEventLength()));
        }

        bool hasColon = arg.Contains(':');
        if (hasColon)
        {
            string[] eventLengthSplit = arg.Split(':');
            if (eventLengthSplit.Length != 2)
            {
                await RespondInvalid();
                return (false, TimeSpan.Zero);
            }
            if (!int.TryParse(eventLengthSplit[0], out int hour) || !int.TryParse(eventLengthSplit[1], out int min))
            {
                await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventLength> invalid values", Constants.Messages.ScheduleHelpEventLength()));
                return (false, TimeSpan.Zero);
            }
            return (true, new TimeSpan(hour, min, 0));
        }
        else
        {
            if (!int.TryParse(arg, out int hour))
            {
                await ctx.RespondAsync(Constants.Messages.ScheduleHelpError("<eventLength> invalid value", Constants.Messages.ScheduleHelpEventLength()));
                return (false, TimeSpan.Zero);
            }
            return (true, new TimeSpan(hour, 0, 0));
        }
    }
}