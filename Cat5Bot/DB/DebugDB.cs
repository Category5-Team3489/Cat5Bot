namespace Cat5Bot.DB;

public static class DebugDB
{
    private readonly static string DebugDBDir = Directory.GetCurrentDirectory() + @"\logs\db\";
    private readonly static List<string> data = new();
    private readonly static StringBuilder sb = new();

    public static void Log(int indention, string message)
    {
        sb.Clear();
        for (int i = 0; i < indention; i++)
            sb.Append('\t');
        data.Add($"{sb}{message}");
    }

    public static void Save()
    {
        Directory.CreateDirectory(DebugDBDir);
        File.WriteAllLines(DebugDBDir + DateTime.Now.ToFileTime() + ".txt", data);
        data.Clear();
    }
}