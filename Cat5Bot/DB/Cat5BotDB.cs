namespace Cat5Bot.DB;

public class Cat5BotDB
{
    #region Singleton
    private Cat5BotDB()
    {

    }
    private static Cat5BotDB? instance = null;
    private static readonly object instanceLock = new();
    public static Cat5BotDB I
    {
        get
        {
            if (instance is null)
            {
                lock (instanceLock)
                {
                    if (instance is null)
                    {
                        instance = new();
                    }
                }
            }
            return instance;
        }
    }
    #endregion Singleton

    #region Public Fields
    public object Lock { get; private set; } = new();
    public NamesDB Names { get; private set; } = new();
    public PermissionsDB Permissions { get; private set; } = new();
    public EventsDB Events { get; private set; } = new();
    public AttendanceDB Attendance { get; private set; } = new();
    #endregion Public Fields

    #region Private Fields
    private readonly static string DBPath = Directory.GetCurrentDirectory() + @"\Cat5Bot.db";
    private readonly DBWriter writer = new();
    #endregion Private Fields

    #region File IO
    public void Load()
    {
        DBReader reader = new();

        if (File.Exists(DBPath))
        {
            byte[] data = File.ReadAllBytes(DBPath);
            reader.SetSource(data);

            Names.Deserialize(reader);
            Permissions.Deserialize(reader);
            Events.Deserialize(reader);
            Attendance.Deserialize(reader);
        }
    }
    public void Save()
    {
        writer.Reset();

        Names.Serialize(writer);
        Permissions.Serialize(writer);
        Events.Serialize(writer);
        Attendance.Serialize(writer);

        byte[] data = writer.CopyData();
        File.WriteAllBytes(DBPath, data);
    }
    #endregion File IO
}