namespace Cat5Bot;

public class Log
{
    #region Singleton
    private Log() { }
    private static Log? instance = null;
    private static readonly object instanceLock = new();
    private static Log I
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

    private const int MaxLogSize = 1000;

    public bool IsDebug { get; set; } = false;

    private readonly static string LogDirPath = Directory.GetCurrentDirectory() + @"\logs\";
    private readonly static string LatestLogPath = LogDirPath + "latest.txt";

    private object Lock { get; set; } = new();
    private List<string> data = new();

    public static void SetMode(bool isDebug)
    {
        I.IsDebug = isDebug;
    }

    public static void Command(string commandName, string commandExecutor, string message = "")
    {
        if (!string.IsNullOrEmpty(message))
            All($"Command \"{commandName}\" executed by \"{commandExecutor}\": {message}");
        else
            All($"Command \"{commandName}\" executed by \"{commandExecutor}\"");
    }

    public static void All(string message)
    {
        lock (I.Lock)
        {
            string log = $"[{DateTime.Now}]: {message}";
            Console.WriteLine(log);
            I.data.Add(log);
            I.SaveIfNeeded();
        }
    }

    public static void Debug(string message)
    {
        lock (I.Lock)
        {
            string log = $"[{DateTime.Now}][Debug]: {message}";
            if (I.IsDebug) Console.WriteLine(log);
            I.data.Add(log);
            I.SaveIfNeeded();
        }
    }

    public static void Load()
    {
        Directory.CreateDirectory(LogDirPath);
        if (File.Exists(LatestLogPath))
        {
            List<string> data = new(File.ReadAllLines(LatestLogPath));
            lock (I.Lock)
            {
                I.data = data;
            }
        }
    }

    public static void Save()
    {
        lock (I.Lock)
        {
            I.Save(false);
        }
    }

    private void SaveIfNeeded()
    {
        if (data.Count >= MaxLogSize)
        {
            Save(true);
            data.Clear();
        }
    }

    private void Save(bool archive)
    {
        if (archive)
            File.WriteAllLines(LogDirPath + DateTime.Now.ToFileTime() + ".txt", data);
        else
            File.WriteAllLines(LatestLogPath, data);
    }
}