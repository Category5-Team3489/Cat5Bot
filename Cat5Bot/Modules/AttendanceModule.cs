namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("attend"), Aliases("a"), Description("Links a user's name to their account.")]
public class AttendanceModule : BaseCommandModule
{
    [GroupCommand, Description("Links a full name to an attendance record.")]
    public async Task AttendSelf(CommandContext ctx)
    {
        Log.Command("attend self", ctx.User.ToString());
        // User must have greater or equal perms than command executor
        await ctx.RespondAsync("Hola?");
    }
}