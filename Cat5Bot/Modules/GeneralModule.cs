namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

public class GeneralModule : BaseCommandModule
{
    [Command("test"), Description("Test command")]
    public async Task Test(CommandContext ctx)
    {
        Log.Command("Test", ctx.Member!.DisplayName);
        await ctx.RespondAsync("Testing!");
    }
}