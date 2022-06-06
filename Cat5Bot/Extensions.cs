using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat5Bot;

public static class Extensions
{
    public static async Task CreateReactionsAsync(this DiscordMessage msg, int delayBetween, params DiscordEmoji[] emojis)
    {
        if (emojis.Length < 1) return;
        else if (emojis.Length == 1)
        {
            await msg.CreateReactionAsync(emojis[0]);
        }
        else
        {
            int last = emojis.Length - 1;
            for (int i = 0; i < last; i++)
            {
                await msg.CreateReactionAsync(emojis[i]);
                await Task.Delay(delayBetween);
            }
            await msg.CreateReactionAsync(emojis[last]);
        }
    }
}