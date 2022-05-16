namespace Cat5BotDB;

public static class Utils
{
    public static string Hash(string str)
    {
        SHA256Managed crypt = new SHA256Managed();
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str));
        StringBuilder hash = new StringBuilder();
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }
}