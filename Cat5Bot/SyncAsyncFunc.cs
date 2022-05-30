using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat5Bot;

public class SyncAsyncFunc
{
    public bool IsSync { get; private set; }
    public Action? SyncFunc { get; private set; }
    public Func<Task>? AsyncFunc { get; private set; }

    public SyncAsyncFunc(Action syncFunc)
    {
        IsSync = true;
        SyncFunc = syncFunc;
    }

    public SyncAsyncFunc(Func<Task> asyncFunc)
    {
        IsSync = false;
        AsyncFunc = asyncFunc;
    }

    public void RunIfSync()
    {
        if (IsSync && SyncFunc is not null)
        {
            SyncFunc();
        }
    }

    public async Task RunIfAsync()
    {
        if (!IsSync && AsyncFunc is not null)
        {
            await AsyncFunc();
        }
    }

    public async Task RunAny()
    {
        RunIfSync();
        await RunIfAsync();
    }
}