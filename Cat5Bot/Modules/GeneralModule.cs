namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

public class GeneralModule : BaseCommandModule
{
    [Command("test"), Description("Test command")]
    public async Task Test(CommandContext ctx)
    {
        Log.Command("test", ctx.User.ToString());
        await ctx.RespondAsync("Testing!");
    }
}