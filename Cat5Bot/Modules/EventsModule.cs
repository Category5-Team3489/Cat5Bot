namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("schedule"), Aliases("s"), Description("Links a user's name to their account.")]
public class EventsModule : BaseCommandModule
{
    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task ScheduleHelp(CommandContext ctx)
    {
        Log.Command("schedule self", ctx.User.ToString());

        await ctx.RespondAsync(
            "syntax: s <eventDate> <eventTime> <eventLength> <eventType> <eventName>\n" +
            "syntax: schedule <eventDate> <eventTime> <eventLength> <eventType> <eventName>\n" +
            "\nformat:\n" +
            "\teventDate: 1/31, 1/31/22, 1/31/2022\n" +
            "\t\tinfo: M, MM, D, DD, YY, YYYY is allowed; no spaces\n" +
            "\teventTime: 5pm, 5:00pm, 5PM, 5:00PM\n" +
            "\t\tinfo: include am, pm; no spaces; assumed EST; no 24h time\n" +
            "\teventLength: 3, 3:00\n" +
            "\t\tinfo: hour is the leading number; no spaces\n" +
            "\teventType: Meeting, Fundraiser, Paintball\n" +
            "\t\tinfo: SINGLE WORD; no spaces\n" +
            "\teventName: Meeting at the shop, Flowertown Festival fundraiser\n" +
            "\t\tinfo: any text; any number of spaces\n" +
            "\nex: s 1/31 5pm 3 Meeting Meeting at the shop\n" +
            "\tinfo: creates a 3 hour \"Meeting\" at 5 pm on 1/31 named \"Meeting at the shop\""
        );
    }

    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task Schedule(CommandContext ctx, params string[] args)
    {

    }
}