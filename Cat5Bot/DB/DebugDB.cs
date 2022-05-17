namespace Cat5Bot.DB;

public static class DebugDB
{
    private readonly static string DebugDBDir = Directory.GetCurrentDirectory() + @"\logs\db\";
    private readonly static StringBuilder sb = new();

    public static void Log(int indention, string message)
    {
        for (int i = 0; i < indention; i++)
            sb.Append('\t');
        sb.Append($"{message}\n");
    }

    public static void Save()
    {
        Directory.CreateDirectory(DebugDBDir);
        string text = sb.ToString();
        File.WriteAllText(DebugDBDir + Utils.Hash(text) + ".txt", text);
        sb.Clear();
    }
}