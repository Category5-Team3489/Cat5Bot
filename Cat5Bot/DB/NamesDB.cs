namespace Cat5Bot.DB;

public class NamesDB : IDBSerializable<NamesDB>
{
    private readonly Dictionary<ulong, string> names = new();

    public bool TryGet(ulong discordId, out string? name)
    {
        return names.TryGetValue(discordId, out name);
    }

    public bool TryGet(string name, out ulong discordId)
    {
        foreach ((ulong oDiscordId, string oName) in names)
        {
            if (oName == name)
            {
                discordId = oDiscordId;
                return true;
            }
        }
        discordId = 0;
        return false;
    }

    public bool TryAdd(ulong discordId, string name)
    {
        return names.TryAdd(discordId, name);
    }

    public void Set(ulong discordId, string name)
    {
        if (names.ContainsKey(discordId))
            names[discordId] = name;
        else
            names.Add(discordId, name);
    }

    public bool TryRemove(ulong discordId, out string? name)
    {
        if (TryGet(discordId, out name))
        {
            return names.Remove(discordId);
        }
        return false;
    }

    public bool TryRemove(string name, out ulong discordId)
    {
        if (TryGet(name, out discordId))
        {
            return names.Remove(discordId);
        }
        discordId = 0;
        return false;
    }

    public NamesDB Deserialize(DBReader reader)
    {
        names.Clear();
        int length = reader.GetInt();
        for (int i = 0; i < length; i++)
        {
            ulong discordId = reader.GetULong();
            string name = reader.GetString();
            names.Add(discordId, name);
        }
        return this;
    }

    public NamesDB Serialize(DBWriter writer)
    {
        DebugDB.Log(1, "Names Start");
        int length = names.Count;
        DebugDB.Log(2, $"Length: {length}");
        writer.Put(length);
        foreach ((ulong discordId, string name) in names)
        {
            DebugDB.Log(3, $"Discord Id: {discordId}");
            writer.Put(discordId);
            DebugDB.Log(3, $"Name: {name}");
            writer.Put(name);
        }
        return this;
    }
}