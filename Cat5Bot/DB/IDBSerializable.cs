namespace Cat5Bot.DB;

public interface IDBSerializable<T>
{
    public T Deserialize(DBReader reader);
    public T Serialize(DBWriter writer);
}