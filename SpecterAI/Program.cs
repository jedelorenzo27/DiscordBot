using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SummaryAttribute = Discord.Commands.SummaryAttribute;

public class Program
{
    private DiscordSocketClient? _client;
    private CommandService? _commands;
    private InteractionService? _interactions;
    private IServiceProvider? _services;

    public static async Task Main(string[] args) => await new Program().MainAsync();

    public async Task MainAsync()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent | GatewayIntents.All
        };

        _client = new DiscordSocketClient(config);
        _commands = new CommandService();
        _interactions = new InteractionService(_client);

        _services = BuildServiceProvider();

        _client.Log += Log;
        _client.MessageReceived += HandleCommandAsync;

        string token = "token here";
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.Ready += Client_Ready;

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    private async Task Client_Ready()
    {
        await _interactions.RegisterCommandsGloballyAsync();
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null || message.Author.IsBot) return;

        int argPos = 0;

        if (!(message.HasCharPrefix('!', ref argPos))) return;

        var context = new SocketCommandContext(_client, message);
        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess)
        {
            Console.WriteLine("error -> " + result.ErrorReason);
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    // Define a method to build your service provider
    private IServiceProvider BuildServiceProvider() => new ServiceCollection()
        // Add other services here
        .AddSingleton(_client)
        .AddSingleton(_commands)
        .AddSingleton(_interactions)
        .BuildServiceProvider();
}

// Define a module for your text-based commands
public class TextCommandModule : ModuleBase<SocketCommandContext>
{
    [Command("echo")]
    [Summary("Echoes the input back to you.")]
    public async Task EchoAsync([Remainder] string input)
    {
        Console.WriteLine($"Echo {input}");
        await ReplyAsync(input);
    }
}

// Define a module to hold your slash command
public class CommandModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("echo", "Echoes the input back to you.")]
    public async Task EchoAsync(string input)
    {
        Console.WriteLine($"Echo {input}");
        await RespondAsync(input);
    }
}
