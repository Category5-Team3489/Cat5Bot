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
            ScheduledEvent? scheduledEventInternal = Cat5BotDB.I.Events.GetClosest();
            if (scheduledEventInternal is not null)
            {
                Cat5BotDB.I.Attendance.Set(ctx.User.Id, scheduledEventInternal.Id);
                scheduledEvent = scheduledEventInternal.Clone();
            }
        }

        if (scheduledEvent is null)
        {
            await ctx.RespondAsync("No recent attendable event found");
            return;
        }

        string attendedEventText = $"Attending \"{scheduledEvent.name}\": {scheduledEvent.Summarize()}";

        var interactivity = ctx.Client.GetInteractivity();
        bool canceled = await CommandHelper.Cancellable(ctx, interactivity, attendedEventText, Constants.AttendedEventCancelTimeout);
        if (canceled)
        {
            lock (Cat5BotDB.I.Lock)
            {
                if (Cat5BotDB.I.Attendance.TryGet(ctx.User.Id, out MemberAttandance? memberAttendance))
                {
                    memberAttendance!.TryRemove(scheduledEvent.Id);
                }
            }
        }
    }
}