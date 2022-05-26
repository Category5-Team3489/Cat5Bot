using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat5Bot.DB;

public class ScheduledEvent : IDBSerializable<ScheduledEvent>, IDBCloneable<ScheduledEvent>
{
    public string name = "";
    public string type = "";
    public DateTime start;
    public double duration;

    public ScheduledEvent() { }

    public ScheduledEvent(string name, string type, DateTime start, double duration)
    {
        this.name = name;
        this.type = type;
        this.start = start;
        this.duration = duration;
    }

    public ScheduledEvent Clone()
    {
        return new ScheduledEvent(name, type, start, duration);
    }

    public ScheduledEvent Deserialize(DBReader reader)
    {
        name = reader.GetString();
        type = reader.GetString();
        start = DateTime.FromFileTimeUtc(reader.GetLong());
        duration = reader.GetDouble();
        return this;
    }

    public ScheduledEvent Serialize(DBWriter writer)
    {
        DebugDB.Log(3, "Scheduled Event Start");
        DebugDB.Log(4, $"Name: {name}");
        DebugDB.Log(4, $"Type: {type}");
        DebugDB.Log(4, $"Start: {start.ToFileTimeUtc()}");
        DebugDB.Log(4, $"Duration: {duration}");
        writer.Put(name);
        writer.Put(type);
        writer.Put(start.ToFileTimeUtc());
        writer.Put(duration);
        return this;
    }

    public ScheduledEvent CloneAsLocal()
    {
        ScheduledEvent scheduledEvent = Clone();
        scheduledEvent.start = start.ToLocalTime();
        return scheduledEvent;
    }

    public string Summarize()
    {
        return $"{type} {start:M/d} at {start:h':'mmtt} for {TimeSpan.FromSeconds(duration):h':'mm}";
    }
}