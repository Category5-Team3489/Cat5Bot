using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat5Bot.DB;

public class ScheduledEvent : IDBSerializable<ScheduledEvent>, IDBCloneable<ScheduledEvent>
{
    public ulong Id { get; private set; }

    public string name = "";
    public string type = "";
    public DateTime start;
    public TimeSpan duration;

    public DateTime End => start + duration;

    public void SetId(ulong id)
    {
        Id = id;
    }

    public ScheduledEvent() { }

    public ScheduledEvent(string name, string type, DateTime start, TimeSpan duration)
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
        duration = TimeSpan.FromSeconds(reader.GetDouble());
        return this;
    }

    public ScheduledEvent Serialize(DBWriter writer)
    {
        DebugDB.Log(3, "Scheduled Event Start");
        DebugDB.Log(4, $"Name: {name}");
        DebugDB.Log(4, $"Type: {type}");
        DebugDB.Log(4, $"Start: {start:M/d/yyyy} at {start:h':'mmtt}");
        DebugDB.Log(4, $"Duration: {duration:h':'mm}");
        writer.Put(name);
        writer.Put(type);
        writer.Put(start.ToFileTimeUtc());
        writer.Put(duration.TotalSeconds);
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
        return $"{type} {start:M/d} at {start:h':'mmtt} for {duration:h':'mm}";
    }
}