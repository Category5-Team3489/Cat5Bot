public class TestModule : BaseCommandModule
{
    [GroupCommand, Description("Test command")]
    public async Task Test(CommandContext ctx)
    {
        await ctx.RespondAsync($"Hi!");
    }
}