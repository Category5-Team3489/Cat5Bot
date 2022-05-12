// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = "ODM1Njc4MzM3MjIxMTMyMjg4.GZeg4k.rUwoqGB24ELSJFdzoG4hTFd-OwJa0vKAoKLfvs",
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

while (true)
{
    if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.X)
        break;
    await Task.Delay(1000);
}