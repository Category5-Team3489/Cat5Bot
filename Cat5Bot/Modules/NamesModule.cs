namespace Cat5Bot.Modules;

#pragma warning disable CA1822 // Mark members as static

[Group("name"), Aliases("n"), Description("Links a user's name to their account.")]
public class NamesModule : BaseCommandModule
{
    [GroupCommand, Description("Links your full name to your attendance record.")]
    public async Task NameSelf(CommandContext ctx, [Description("Your full name")] params string[] fullName)
    {
        ulong id = ctx.User.Id;
        string name = string.Join(' ', fullName);
        if (string.IsNullOrWhiteSpace(name))
        {
            await ctx.RespondAsync($"Please supply your name.");
            return;
        }
        lock (Cat5BotDB.I.Lock)
        {
            Cat5BotDB.I.Names.Set(id, name);
        }
        await ctx.RespondAsync($"Your name is now set to \"{name}\" in the database.");
    }

    [GroupCommand, Description("Links a full name to an attendance record.")]
    public async Task NameOther(CommandContext ctx, [Description("User")] DiscordUser user, [Description("Their full name")] params string[] fullName)
    {
        // TODO User must have greater or equal perms than command executor

        ulong id = user.Id;
        string name = string.Join(' ', fullName);
        if (string.IsNullOrWhiteSpace(name))
        {
            await ctx.RespondAsync($"Please supply their name.");
            return;
        }
        lock (Cat5BotDB.I.Lock)
        {
            Cat5BotDB.I.Names.Set(id, name);
        }
        await ctx.RespondAsync($"Their name is now set to \"{name}\" in the database.");
    }
}