namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("attend"), Aliases("a"), Description("Links a user's name to their account.")]
public class AttendanceModule : BaseCommandModule
{
    [GroupCommand, Description("Links a full name to an attendance record.")]
    public async Task AttendSelf(CommandContext ctx)
    {
        Log.Command("attend self", ctx.User.ToString());

        ScheduledEvent? scheduledEvent = null;
        lock (Cat5BotDB.I.Lock)
        {
            scheduledEvent = Cat5BotDB.I.Events.GetClosest();
            if (scheduledEvent is not null)
            {
                Cat5BotDB.I.Attendance.Set(ctx.User.Id, scheduledEvent.Id);
            }
        }

        if (scheduledEvent is null)
        {
            await ctx.RespondAsync("No recent attendable event found");
            return;
        }

        string attendedEventText = $"Attending \"{scheduledEvent.name}\": {scheduledEvent.Summarize()}";

        var interactivity = ctx.Client.GetInteractivity();
        var msg = await ctx.RespondAsync(
            $"{attendedEventText}\n" +
            $"Hit the \"X\" to cancel, timeout in {Constants.AttendingEventCancelTimeout}s"
        );
        var x = DiscordEmoji.FromName(ctx.Client, ":x:");
        await msg.CreateReactionAsync(x);
        bool cancelled = !(await interactivity.WaitForReactionAsync(xe => xe.Emoji == x && xe.Message == msg, ctx.User, TimeSpan.FromSeconds(Constants.ScheduledEventCancelTimeout))).TimedOut;
        if (cancelled)
        {
            lock (Cat5BotDB.I.Lock)
            {
                if (Cat5BotDB.I.Attendance.TryGet(ctx.User.Id, out MemberAttandance? memberAttendance))
                {
                    memberAttendance!.TryRemove(scheduledEvent.Id);
                }
            }
            await msg.DeleteAsync();
        }
        else
        {
            await msg.DeleteOwnReactionAsync(x);
            await msg.ModifyAsync(attendedEventText);
        }
    }
}