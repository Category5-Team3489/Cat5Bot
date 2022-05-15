namespace Cat5Bot.DB;

public class MemberAttandance : IDBSerializable<MemberAttandance>
{
    private readonly HashSet<ulong> attendedEvents = new();

    public List<ulong> Get()
    {
        return new List<ulong>(attendedEvents);
    }

    public void Set(ulong eventId)
    {
        attendedEvents.Add(eventId);
    }

    public bool TryRemove(ulong eventId)
    {
        return attendedEvents.Remove(eventId);
    }

    public MemberAttandance Deserialize(DBReader reader)
    {
        attendedEvents.Clear();
        int length = reader.GetInt();
        for (int i = 0; i < length; i++)
        {
            ulong eventId = reader.GetULong();
            attendedEvents.Add(eventId);
        }
        return this;
    }

    public MemberAttandance Serialize(DBWriter writer)
    {
        int length = attendedEvents.Count;
        writer.Put(length);
        foreach (ulong eventId in attendedEvents)
        {
            writer.Put(eventId);
        }
        return this;
    }
}