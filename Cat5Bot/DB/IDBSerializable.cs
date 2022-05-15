namespace Cat5Bot.DB;

public interface IDBSerializable<T>
{
    public T Serialize(DBWriter writer);
    public T Deserialize(DBReader reader);
}