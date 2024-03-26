using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SummaryAttribute = Discord.Commands.SummaryAttribute;
using SpecterAI.pokemonStuff;
using SpecterAI.services;
using System.Net;
using DiscordBot.services;
using DiscordBot;
using System.Data.Common;
using BotDataAccess;
using Microsoft.Data.SqlClient;
using BotDataAccess.repositories;
using DiscordBot.Utilities;

public class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;
    private InteractionService _interactions;
    private IServiceProvider _services;
    public static HttpClient _httpClient;
    public static async Task Main(string[] args) => await new Program().MainAsync();

    public async Task MainAsync()
    {
        HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
        handler.UseDefaultCredentials = true;
        handler.UseProxy = false;
        _httpClient = new HttpClient(handler);
        SecretsHandler.InitializeConfiguration();
        PermissionsService.LoadPermissions();
        await ShameTrainServices.JumpStartShameTrain();

        var configuration = DbConfiguration.BuildConfiguration();
        DbConfiguration.InitializeDatabase(configuration); // Pass the built configuration to initialize the database.

        var connectionString = DbConfiguration.GetDatabaseConnectionString(configuration);
        var dbConnection = new SqlConnection(connectionString);

        var challangeRepo = new ChallengeRepo(connectionString);


        
        // This sets up the bot's basic settings.
        // By choosing "GatewayIntents.All", we're asking to get all types of updates from Discord,
        // like messages, new members, etc.
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
        };

        // Create the main bot client with the settings we just defined.
        // This client handles connections to Discord and manages interactions.
        _client = new DiscordSocketClient(config);

        // Set up a service to manage text-based commands.
        // This allows the bot to respond to specific text commands typed by users.
        _commands = new CommandService();

        // Initialize a service for handling new types of interactions, like slash commands.
        // This connects our bot's commands with Discord's newer interaction system.
        _interactions = new InteractionService(_client);

        // This creates a collection of services that the bot can use.
        // It's like setting up a toolbox where the bot can find all the tools (services)
        // it needs to work properly.
        _services = BuildServiceProvider();


        // Listen for log messages from the Discord client and handle them using the Log method.
        // This is useful for debugging and tracking what the bot is doing.
        _client.Log += Log;

         // Set up an event listener for when messages are sent in Discord.
        // When a message is received, it triggers the HandleCommandAsync method to see if it's a command the bot should respond to.
        _client.MessageReceived += HandleCommandAsync;

        // Listen for when users interact with the bot using new interaction features (like slash commands).
        // These interactions trigger the HandleInteractionAsync method, allowing the bot to process and respond to them.
        _client.InteractionCreated += HandleInteractionAsync;


        string token = SecretsHandler.DiscordToken().Result;
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Load and register all text command modules from the current assembly.
        // This makes sure the bot knows about any text commands you've created so it can respond to them.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Load and register all interaction command modules (like slash commands) from the current assembly.
        // This lets the bot recognize and handle slash commands and other interactions.
        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);


        // Set up an event listener for when the bot has successfully connected to Discord and is ready to start processing events.
        // When the bot is ready, it triggers the Client_Ready method, which can include tasks like registering commands with Discord.
        _client.Ready += Client_Ready;

        // Prepare logging service. Any logs sent before here will be printed to console but not sent to discord.
        LoggingService.LoadLoggingServers(_client);

        await LoggingService.LogMessage(LogLevel.Info, $"Starting bot with version: {Constants.BotVersion}"); ;

        // Block this task until the program is closed.
        await Task.Delay(-1);
        // Below breaks cloud deployment
        //Console.ReadKey();
        await _client.LogoutAsync();
    }

    private async Task Client_Ready()
    {
        // Register slash commands
        await _interactions.RegisterCommandsGloballyAsync();
    }

    // Handles non-slash commands
    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null || message.Author.IsBot) return;

        int argPos = 0;

        // 'argPos' is a reference variable used to track the position in the message where the '!' is found.
        if (!(message.HasCharPrefix('!', ref argPos))) return;


        // Create a new context for executing commands based on the received message.
        // This context includes information like which server and channel the message was sent in, and who sent it.
        var context = new SocketCommandContext(_client, message);

        // Run the command from the message, skipping the '!' prefix.
        // 'argPos' tells us where the command begins. '_services' are extra tools the command might use.
        // 'result' tells us if the command worked or not.
        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess)
        {
            Console.WriteLine("error -> " + result.ErrorReason);
        }
    }

    // Handle an interaction (like a slash command) from Discord.
    private async Task HandleInteractionAsync(SocketInteraction interaction)
    {
        // Create a context from the interaction. This includes who triggered the interaction and where it happened.
        var context = new SocketInteractionContext(_client, interaction);

        // Run the interaction command with its context and services, then send the response to Discord.
        await _interactions.ExecuteCommandAsync(context, _services);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    // This method sets up a container for all the tools and services the bot uses.
    private IServiceProvider BuildServiceProvider() => new ServiceCollection()
        // Here, we're telling the container about the different pieces of our bot:
        // the main bot client, the command handler, and the interactions handler
        .AddSingleton(_client) // The bot's connection to Discord.
        .AddSingleton(_commands) // Handles text commands.
        .AddSingleton(_interactions) // Manages slash commands and other interactions.
        .BuildServiceProvider(); // Creates the container with everything set up.
}

/*
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
*/