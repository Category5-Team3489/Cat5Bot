Log.All("[App] Hello, World!");

lock (Cat5BotDB.I.Lock)
{
    Cat5BotDB.I.Load();
}

Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
    e.Cancel = true;
    Console.WriteLine("Press \"X\" to close");
};

static void Save(bool archive)
{
    Log.All("[App] Saving!");
    if (archive)
        Log.All("[App] Archiving!");

    lock (Cat5BotDB.I.Lock)
    {
        Cat5BotDB.I.Save();
    }
    Log.Save(archive);
}

string token = File.ReadAllText(Directory.GetCurrentDirectory() + @"\token.secret");

var discord = new DiscordClient(new DiscordConfiguration()
{
    Token = token,
    TokenType = TokenType.Bot,
    Intents = DiscordIntents.AllUnprivileged,
    MinimumLogLevel = LogLevel.Information
});

discord.UseInteractivity();

var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
{
    StringPrefixes = new[] { "!" },
    EnableDms = false, // If making true, check where accessing member, will be null in dms
});

commands.RegisterCommands<GeneralModule>();
commands.RegisterCommands<NamesModule>();

await discord.ConnectAsync();

ulong loops = 0;
while (true)
{
    if (Console.KeyAvailable)
    {
        ConsoleKey key = Console.ReadKey().Key;
        if (key == ConsoleKey.X)
            break;
        else if (key == ConsoleKey.S)
            Save(false);
    }

    if (loops % (Constants.SavePeriod * 10) == 0)
    {
        Save(false);
    }

    await Task.Delay(100);
    loops++;
}

Log.All("[App] Stopping!");
Save(true);