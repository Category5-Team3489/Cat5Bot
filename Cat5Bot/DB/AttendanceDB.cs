namespace Cat5Bot.DB;

public class AttendanceDB : IDBSerializable<AttendanceDB>
{
    private readonly Dictionary<ulong, MemberAttandance> attendance = new();

    public bool TryGet(ulong discordId, out MemberAttandance? memberAttandance)
    {
        return attendance.TryGetValue(discordId, out memberAttandance);
    }

    public void Set(ulong discordId, ulong eventId)
    {
        if (attendance.ContainsKey(discordId))
            attendance[eventId].Set(eventId);
        else
        {
            MemberAttandance memberAttandance = new();
            memberAttandance.Set(eventId);
            attendance.Add(discordId, memberAttandance);
        }
    }

    public bool TryRemove(ulong discordId, out MemberAttandance? memberAttandance)
    {
        if (TryGet(discordId, out memberAttandance))
        {
            return attendance.Remove(discordId);
        }
        return false;
    }

    public AttendanceDB Deserialize(DBReader reader)
    {
        attendance.Clear();
        int length = reader.GetInt();
        for (int i = 0; i < length; i++)
        {
            ulong discordId = reader.GetULong();
            MemberAttandance memberAttendance = new MemberAttandance().Deserialize(reader);
            attendance.Add(discordId, memberAttendance);
        }
        return this;
    }

    public AttendanceDB Serialize(DBWriter writer)
    {
        DebugDB.Log(1, "Permissions Start");
        int length = attendance.Count;
        DebugDB.Log(2, $"Length: {length}");
        writer.Put(length);
        foreach ((ulong discordId, MemberAttandance memberAttandance) in attendance)
        {
            writer.Put(discordId);
            DebugDB.Log(3, $"Discord Id: {discordId}");
            memberAttandance.Serialize(writer);
        }
        return this;
    }
}