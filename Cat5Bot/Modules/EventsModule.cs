namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("schedule"), Aliases("s"), Description("Links a user's name to their account.")]
public class EventsModule : BaseCommandModule
{
    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task Schedule(CommandContext ctx)
    {

    }
}