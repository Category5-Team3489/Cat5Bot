﻿namespace Cat5Bot.DB;

public class EventsDB : IDBSerializable<EventsDB>
{
    private ulong nextId = 0;
    private readonly Dictionary<ulong, ScheduledEvent> events = new();

    public bool TryGet(ulong eventId, out ScheduledEvent? scheduledEvent)
    {
        return events.TryGetValue(eventId, out scheduledEvent);
    }

    public ulong Add(ScheduledEvent scheduledEvent)
    {
        ulong id = nextId++;
        events.Add(id, scheduledEvent);
        return id;
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
            events.Add(eventId, scheduledEvent);
        }
        return this;
    }

    public EventsDB Serialize(DBWriter writer)
    {
        int length = events.Count;
        writer.Put(nextId);
        writer.Put(length);
        foreach ((ulong eventId, ScheduledEvent scheduledEvent) in events)
        {
            writer.Put(eventId);
            scheduledEvent.Serialize(writer);
        }
        return this;
    }
}