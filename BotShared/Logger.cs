using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BotShared
{
    public enum LogLevel
    {
        DBExecute,
        Action,
        Info,
        Debug,
        ConfigError,
        Error
    }


    public static class Logger
    {
        private static string debug_channels_delimiter = "#";


        private static bool _ready = false;
        private static DiscordSocketClient? _discord;
        private static List<string> _debugChannels = new List<string>();

        /// <summary>
        /// Convenience method to format command-taken logs
        /// </summary>
        /// <returns></returns>
        public static async Task LogCommandUse(SocketInteractionContext context, string command)
        {
            await LogMessage(LogLevel.Action, $"{context.User.Id} used {command} in {context.Channel.Id} ({context.Guild.Id})");

        }
        public static async Task LogCommandUse(SocketInteractionContext context, string command, string target)
        {
            string target_string = target;
            if (context.User.Id.ToString() == target)
            {
                target_string = "self";
            }
            await LogMessage(LogLevel.Action, $"{context.User.Id} used {command} in channel:{context.Channel.Id} (server:{context.Guild.Id}) on {target_string}");
        }



        public static async Task LogMessage(LogLevel level, string[] messages, bool localOnly = false)
        {
            await LogMessage(level, string.Join("\n", messages));
        }



        public static async Task LogMessage(LogLevel level, string message, bool localOnly = false)
        {
            if (_ready)
            {
                if (_debugChannels.Count == 0)
                {
                    Console.WriteLine("!!! No debug servers configured. Create resources/logging/debug_channels.txt if it's not already created.");
                    Console.WriteLine($"Then add servers using this format ChannelId{debug_channels_delimiter}Description");
                    Console.WriteLine($"Where channel id and description are separated by '{debug_channels_delimiter}'");
                    return;
                }

                foreach (string channelId in _debugChannels)
                {
                    try
                    {
                        await sendMessageToChannel(ulong.Parse(channelId), $"[{level}] {message}", localOnly);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send log message to discord channel ({channelId})");
                        Console.WriteLine($"Failed message: \"{message}\"");
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("Trying to log message before logger is ready");
                Console.WriteLine($"This message was not successfully logged: \"{message}\"");
            }
        }

        private static async Task sendMessageToChannel(ulong channelId, string message, bool localOnly)
        {
            Console.WriteLine(message);
            if (!localOnly)
            {
                var chnl = _discord.GetChannel(channelId) as IMessageChannel;
                await chnl.SendMessageAsync(message);
            }
        }

        public static void LoadLoggingServers(DiscordSocketClient client, List<string> debugChannels)
        {
            _debugChannels = debugChannels;
            _discord = client;
            _ready = true;
        }
    }
}
