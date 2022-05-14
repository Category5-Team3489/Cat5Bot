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


}