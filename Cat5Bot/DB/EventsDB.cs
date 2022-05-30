namespace Cat5Bot.DB;

public class EventsDB : IDBSerializable<EventsDB>
{
    private ulong nextId = 0;
    private readonly Dictionary<ulong, ScheduledEvent> events = new();

    public bool TryGet(ulong eventId, out ScheduledEvent? scheduledEvent)
    {
        return events.TryGetValue(eventId, out scheduledEvent);
    }

    public bool TryRemove(ulong eventId, out ScheduledEvent? scheduledEvent)
    {
        if (TryGet(eventId, out scheduledEvent))
        {
            return events.Remove(eventId);
        }
        return false;
    }

    public ulong Add(ScheduledEvent scheduledEvent)
    {
        ulong id = nextId++;
        scheduledEvent.SetId(id);
        events.Add(id, scheduledEvent);
        return id;
    }

    public bool AnyOverlap(ScheduledEvent scheduledEvent)
    {
        return events.Select(e => e.Value)
            .Any(e => e.start <= scheduledEvent.End && e.End >= scheduledEvent.start && e != scheduledEvent);
    }

    public ScheduledEvent? GetClosest()
    {
        DateTime now = DateTime.UtcNow;
        return events.Select(e => e.Value)
            .MinBy(e => Math.Abs((now - e.start).TotalSeconds));
    }

    public IEnumerable<ScheduledEvent> GetFuture()
    {
        DateTime now = DateTime.UtcNow;
        return events.Select(e => e.Value)
            .Where(e => e.start >= now)
            .OrderBy(e => e.start);
    }

    public IEnumerable<ScheduledEvent> GetPast()
    {
        DateTime now = DateTime.UtcNow;
        return events.Select(e => e.Value)
            .Where(e => e.start < now)
            .OrderBy(e => e.start);
    }

    public EventsDB Deserialize(DBReader reader)
    {
        events.Clear();
        nextId = reader.GetULong();
        int length = reader.GetInt();
        for (int i = 0; i < length; i++)
        {
            ulong eventId = reader.GetULong();
            ScheduledEvent scheduledEvent = new ScheduledEvent().Deserialize(reader);
            scheduledEvent.SetId(eventId);
            events.Add(eventId, scheduledEvent);
        }
        return this;
    }

    public EventsDB Serialize(DBWriter writer)
    {
        DebugDB.Log(1, "Events Start");
        int length = events.Count;
        DebugDB.Log(2, $"Next Id: {nextId}");
        DebugDB.Log(2, $"Length: {length}");
        writer.Put(nextId);
        writer.Put(length);
        foreach ((ulong eventId, ScheduledEvent scheduledEvent) in events)
        {
            DebugDB.Log(3, $"Event Id: {eventId}");
            writer.Put(eventId);
            scheduledEvent.Serialize(writer);
        }
        return this;
    }
}