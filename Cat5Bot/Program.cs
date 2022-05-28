Log.All("[App] Hello, World!");

// TODO add event archiving?, or more efficent search in eventsDB?

// TODO add discriptions to commands and anything else

// TODO deduplicate command cancellation code

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
    EnableDms = true, // If making true, check where accessing member, will be null in dms
});

commands.RegisterCommands<GeneralModule>();
commands.RegisterCommands<NamesModule>();
commands.RegisterCommands<AttendanceModule>();
commands.RegisterCommands<EventsModule>();

await discord.ConnectAsync();

await Task.Delay(5000);

async Task SetStatus(string status)
{
    DiscordActivity activity = new(status);
    await discord!.UpdateStatusAsync(activity);
}

async Task SetStatusToNextEvent()
{
    string status = "No events scheduled!";
    lock (Cat5BotDB.I.Lock)
    {
        ScheduledEvent? scheduledEvent = Cat5BotDB.I.Events.GetFuture().FirstOrDefault();
        if (scheduledEvent is not null)
        {
            status = scheduledEvent.CloneAsLocal().Summarize();
        }
    }
    await SetStatus(status);
}

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
        else if (key == ConsoleKey.A)
        {
            await SetStatus("Debug status!");
            Log.All("[App] Debug set status!");
        }
        else if (key == ConsoleKey.P)
        {
            await SetStatusToNextEvent();
            Log.All("[App] Debug set status to next event!");
        }
    }

    if (loops % (Constants.SavePeriod * 10) == 0)
    {
        Save(false);
    }

    if (loops % (Constants.UpdateStatusPeriod * 10) == 0)
    {
        await SetStatusToNextEvent();
    }

    await Task.Delay(100);
    loops++;
}

Log.All("[App] Stopping!");
Save(true);