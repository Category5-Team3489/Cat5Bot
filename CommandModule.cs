public class TestModule : BaseCommandModule
{
    public List<ScheduledEvent> events = new List<ScheduledEvent>();

    [Command("test"), Description("Test command")]
    public async Task Test(CommandContext ctx)
    {
        Console.WriteLine("testing");
        await ctx.RespondAsync("Hi!");
    }

    // !schedule 5/12 5:00pm 3:00 Test Meeting

    [Command("schedule"), Description("Test command")]
    public async Task Schedule(CommandContext ctx, string startDate, string startTime, double length, params string[] name)
    {
        // Name
        string nameJoined = string.Join(" ", name);

        // Start time
        string[] monthAndDay = startDate.Split("/");
        int month = int.Parse(monthAndDay[0]);
        int day = int.Parse(monthAndDay[1]);

        string[] hourAndMin = startTime.Split(":");
        int hour = int.Parse(hourAndMin[0]);
        int min = int.Parse(hourAndMin[1]);

        DateTime date = new DateTime(DateTime.Now.Year, month, day, hour, min, 0);



        ScheduledEvent scheduledEvent = new ScheduledEvent(nameJoined, date, length);
        events.Add(scheduledEvent);

        await ctx.RespondAsync(scheduledEvent.ToString());
    }

    [Command("attend"), Description("Attend command")]
    public async Task Attend(CommandContext ctx)
    {
        Console.WriteLine("attend command running");
        ulong id = ctx.Member.Id;


        
        await ctx.RespondAsync("Discord id: " + id);
    }

    public ScheduledEvent GetClosestEvent()
    {
        DateTime now = DateTime.Now;

        DateTime 
        ScheduledEvent closest = null;

        foreach (ScheduledEvent scheduledEvent in events)
        {
            if (scheduledEvent.)
            scheduledEvent.start
        }
    }
}