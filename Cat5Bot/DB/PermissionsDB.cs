namespace Cat5Bot.DB;

public class PermissionsDB : IDBSerializable<PermissionsDB>
{
    private readonly Dictionary<ulong, byte> permissions = new();

    public bool TryGet(ulong discordId, out byte permissionLevel)
    {
        return permissions.TryGetValue(discordId, out permissionLevel);
    }

    public void Set(ulong discordId, byte permissionLevel)
    {
        if (permissions.ContainsKey(discordId))
            permissions[discordId] = permissionLevel;
        else
            permissions.Add(discordId, permissionLevel);
    }

    public bool TryRemove(ulong discordId, out byte permissionLevel)
    {
        if (TryGet(discordId, out permissionLevel))
        {
            return permissions.Remove(discordId);
        }
        return false;
    }

    public PermissionsDB Deserialize(DBReader reader)
    {
        permissions.Clear();
        int length = reader.GetInt();
        for (int i = 0; i < length; i++)
        {
            ulong discordId = reader.GetULong();
            byte permissionLevel = reader.GetByte();
            permissions.Add(discordId, permissionLevel);
        }
        return this;
    }

    public PermissionsDB Serialize(DBWriter writer)
    {
        DebugDB.Log(1, "Permissions Start");
        int length = permissions.Count;
        DebugDB.Log(2, $"Length: {length}");
        writer.Put(length);
        foreach ((ulong discordId, byte permissionLevel) in permissions)
        {
            DebugDB.Log(3, $"Discord Id: {discordId}");
            writer.Put(discordId);
            DebugDB.Log(3, $"Permission Level: {permissionLevel}");
            writer.Put(permissionLevel);
        }
        return this;
    }
}