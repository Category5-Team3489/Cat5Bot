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

    [Command("select"), Description("Links your full name to your attendance record.")]
    public async Task ScheduleSelect(CommandContext ctx)
    {
        try
        {
            Log.Command("schedule select", ctx.User.ToString());

            List<ScheduledEvent> scheduledEvents;
            lock (Cat5BotDB.I.Lock)
            {
                scheduledEvents = Cat5BotDB.I.Events.GetAllOrdered().Select(e => e.Clone()).ToList();
            }

            int selectedIndex = -1;
            ScheduledEvent? closest = null;
            double closestTime = double.MaxValue;
            for (int i = 0; i < scheduledEvents.Count; i++)
            {
                ScheduledEvent scheduledEvent = scheduledEvents[i];
                double closeness = Math.Abs((DateTime.UtcNow - scheduledEvent.start).TotalSeconds);
                if (closest == null || closeness < closestTime)
                {
                    selectedIndex = i;
                    closest = scheduledEvent;
                    closestTime = closeness;
                }
            }

            if (closest is null)
            {
                // TODO NO CLOSEST
                return;
            }

            List<ScheduledEvent> GetNext5(int index)
            {
                if (index < 0)
                {
                    index = 0;
                }
                else if (index >= scheduledEvents.Count)
                {
                    index = scheduledEvents.Count - 1;
                }

                Range next5 = index..(index + 5);
                if (next5.End.Value > scheduledEvents.Count)
                {
                    next5 = new Range(next5.Start, scheduledEvents.Count);
                }

                return scheduledEvents.Take(next5).ToList();
            }

            string GetNext5Text(int index)
            {
                StringBuilder list = new("");

                int number = 1;
                List<ScheduledEvent> next5 = GetNext5(index);
                foreach (ScheduledEvent scheduledEvent in next5)
                {
                    list.Append($"\t{number}. \"{scheduledEvent.name}\": {scheduledEvent.CloneAsLocal().Summarize()}\n");
                    number++;
                }

                return list.ToString();
            }

            var msg = await ctx.RespondAsync(GetNext5Text(selectedIndex));

            var eBackward = DiscordEmoji.FromName(ctx.Client, ":arrow_backward:");
            var eForward = DiscordEmoji.FromName(ctx.Client, ":arrow_forward:");
            var e1 = DiscordEmoji.FromName(ctx.Client, ":one:");
            var e2 = DiscordEmoji.FromName(ctx.Client, ":two:");
            var e3 = DiscordEmoji.FromName(ctx.Client, ":three:");
            var e4 = DiscordEmoji.FromName(ctx.Client, ":four:");
            var e5 = DiscordEmoji.FromName(ctx.Client, ":five:");

            List<DiscordEmoji> emojis = new() { eBackward, eForward, e1, e2, e3, e4, e5 };
            await msg.CreateReactionsAsync(300, emojis.ToArray());

            var interactivity = ctx.Client.GetInteractivity();

            while (true)
            {
                var em = await interactivity.WaitForReactionAsync(xe => emojis.Contains(xe.Emoji) && xe.Message == msg, ctx.User, TimeSpan.FromSeconds(Constants.SchedulingSelectionTimeout));

                if (em.TimedOut)
                {
                    break;
                }

                await msg.DeleteReactionAsync(em.Result.Emoji, ctx.User);

                if (em.Result.Emoji == eBackward)
                {
                    selectedIndex -= 5;
                    await msg.ModifyAsync(GetNext5Text(selectedIndex));
                }
                else if (em.Result.Emoji == eForward)
                {
                    selectedIndex += 5;
                    await msg.ModifyAsync(GetNext5Text(selectedIndex));
                }
                else if (em.Result.Emoji == e1)
                {
                    if (selectedIndex >= 0 && selectedIndex < scheduledEvents.Count)
                    {
                        ScheduledEvent scheduledEvent = scheduledEvents[selectedIndex];
                        await msg.ModifyAsync(scheduledEvent.CloneAsLocal().Summarize());
                    }
                    else
                    {
                        await msg.ModifyAsync("Invalid");
                    }
                }
                else if (em.Result.Emoji == e2)
                {
                    if (selectedIndex >= 0 && selectedIndex < scheduledEvents.Count - 1)
                    {
                        ScheduledEvent scheduledEvent = scheduledEvents[selectedIndex + 1];
                        await msg.ModifyAsync(scheduledEvent.CloneAsLocal().Summarize());
                    }
                    else
                    {
                        await msg.ModifyAsync("Invalid");
                    }
                }
                else if (em.Result.Emoji == e3)
                {
                    if (selectedIndex >= 0 && selectedIndex < scheduledEvents.Count - 2)
                    {
                        ScheduledEvent scheduledEvent = scheduledEvents[selectedIndex + 2];
                        await msg.ModifyAsync(scheduledEvent.CloneAsLocal().Summarize());
                    }
                    else
                    {
                        await msg.ModifyAsync("Invalid");
                    }
                }
                else if (em.Result.Emoji == e4)
                {
                    if (selectedIndex >= 0 && selectedIndex < scheduledEvents.Count - 3)
                    {
                        ScheduledEvent scheduledEvent = scheduledEvents[selectedIndex + 3];
                        await msg.ModifyAsync(scheduledEvent.CloneAsLocal().Summarize());
                    }
                    else
                    {
                        await msg.ModifyAsync("Invalid");
                    }
                }
                else if (em.Result.Emoji == e5)
                {
                    if (selectedIndex >= 0 && selectedIndex < scheduledEvents.Count - 4)
                    {
                        ScheduledEvent scheduledEvent = scheduledEvents[selectedIndex + 4];
                        await msg.ModifyAsync(scheduledEvent.CloneAsLocal().Summarize());
                    }
                    else
                    {
                        await msg.ModifyAsync("Invalid");
                    }
                }
            }
            await msg.ModifyAsync("Timeout");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
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
                list.Append($"\t{index}. \"{scheduledEvent.name}\": {scheduledEvent.CloneAsLocal().Summarize()}\n");
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

        string scheduledEventText = $"Created \"{scheduledEvent.name}\": {scheduledEvent.Summarize()}";
        if (eventStart < DateTime.UtcNow)
        {
            scheduledEventText += $"\nYou created this event in the past, was that intentional?";
        }

        ulong eventId;
        lock (Cat5BotDB.I.Lock)
        {
            if (Cat5BotDB.I.Events.AnyOverlap(scheduledEvent))
            {
                scheduledEventText += $"\nThis event overlaps another event, was that intentional?";
            }

            eventId = Cat5BotDB.I.Events.Add(scheduledEvent);
        }

        var interactivity = ctx.Client.GetInteractivity();
        bool canceled = await CommandHelper.Cancellable(ctx, interactivity, scheduledEventText, Constants.ScheduledEventCancelTimeout);
        if (canceled)
        {
            lock (Cat5BotDB.I.Lock)
            {
                Cat5BotDB.I.Events.TryRemove(eventId, out _);
            }
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