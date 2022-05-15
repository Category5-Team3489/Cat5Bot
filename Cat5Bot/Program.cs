Log.Load();

Log.All("Hello, World!");

string token = File.ReadAllText(Directory.GetCurrentDirectory() + @"\token.secret");

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged
});

discord.UseInteractivity();

var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
{
    StringPrefixes = new[] { "!" }
});

commands.RegisterCommands<TestModule>();

await discord.ConnectAsync();

ulong loops = 0;
while (true)
{
    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.X)
        break;

    if (loops % 100 == 0)
        Log.Save();

    await Task.Delay(1000);
    loops++;
}