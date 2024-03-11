using Discord;
using Discord.WebSocket;
using SpecterAI.Utilities;

namespace SpecterAI.services
{
    public enum LogLevel
    {
        Info,
        Debug,
        Error
    }


    public static class LoggingService
    {
        private static string debug_channels_delimiter = "#";
        private static string debug_channels_file
        {
            get
            {
                return GeneralUtilities.resourceDirectory + "logging" + Path.DirectorySeparatorChar + "debug_channels.txt";
            }
        }


        private static bool _ready = false;
        private static DiscordSocketClient? _discord;
        private static List<string> _debugChannels = new List<string>();

        public static async Task LogMessage(LogLevel level, string message)
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

                foreach(string channelId in _debugChannels)
                {
                    try
                    {
                        await sendMessageToChannel(ulong.Parse(channelId), $"[{level}]{message}");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send log message to discord channel ({channelId})");
                        Console.WriteLine($"Failed message: \"{message}\"");
                        Console.WriteLine(ex.ToString());
                    }
                }
            } else
            {
                Console.WriteLine("Trying to log message before logger is ready");
                Console.WriteLine($"This message was not successfully logged: \"{message}\"");
            }
        }

        private static async Task sendMessageToChannel(ulong channelId, string message)
        {
            var chnl = _discord.GetChannel(channelId) as IMessageChannel;
            await chnl.SendMessageAsync(message);
            Console.WriteLine(message);
        }

        public static void LoadLoggingServers (DiscordSocketClient client)
        {
            try
            {
                if (File.Exists(debug_channels_file))
                {
                    string[] lines = File.ReadAllLines(debug_channels_file);
                    foreach (string line in lines)
                    {
                        if (!line.StartsWith("#")) {
                            string[] split = line.Split(debug_channels_delimiter);
                            string channel_id = split[0].Trim();
                            _debugChannels.Add(channel_id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something seriously bingo bongo's while loading banned users file");
                Console.WriteLine(ex.Message);
            }

            _discord = client;
            _ready = true;
        }
    }
}
