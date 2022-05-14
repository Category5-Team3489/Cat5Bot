namespace Cat5Bot.DB;

public interface IDBSerializable
{
    public void Serialize(DBWriter writer);
    public void Deserialize(DBReader reader);
}