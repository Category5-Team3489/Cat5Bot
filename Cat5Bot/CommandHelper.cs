namespace Cat5Bot;

public static class CommandHelper
{
    public static async Task<bool> Cancellable(CommandContext ctx, InteractivityExtension interactivity, string text, int timeout)
    {
        var msg = await ctx.RespondAsync(
            $"{text}\n" +
            $"Hit the \"X\" to cancel, timeout in {timeout}s"
        );
        var x = DiscordEmoji.FromName(ctx.Client, ":x:");
        await msg.CreateReactionAsync(x);
        bool cancelled = !(await interactivity.WaitForReactionAsync(xe => xe.Emoji == x && xe.Message == msg, ctx.User, TimeSpan.FromSeconds(Constants.ScheduledEventCancelTimeout))).TimedOut;
        if (cancelled)
        {
            await msg.DeleteAsync();
        }
        else
        {
            await msg.DeleteOwnReactionAsync(x);
            await msg.ModifyAsync(text);
        }
        return cancelled;
    }
}