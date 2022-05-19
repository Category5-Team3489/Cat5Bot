namespace Cat5Bot;

public static class Utils
{
    public static string Hash(string str)
    {
        StringBuilder hash = new();
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(str));
            foreach (byte b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }
        }
        return hash.ToString();
    }
}