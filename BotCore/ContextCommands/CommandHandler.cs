using Discord.Commands;
using BotTemplate.BotCore.ContextCommands.Results;

namespace BotTemplate.BotCore.ContextCommands;

/// <summary>Handles the processing and execution of traditional text-based commands.
/// This is separate from the Interaction framework and handles commands that start with a prefix (e.g., !help).</summary>
public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandler> _logger;

    // Command prefix - you might want to make this configurable through your configuration system
    private const string CommandPrefix = "!";

    public CommandHandler(
        DiscordSocketClient client,
        CommandService commands,
        IServiceProvider services,
        ILogger<CommandHandler> logger)
    {
        _client = client;
        _commands = commands;
        _services = services;
        _logger = logger;
    }

    /// <summary>Initializes the command handler by registering message handling.
    /// Should be called after the client is ready.</summary>
    public Task InitializeAsync()
    {
        // Hook MessageReceived so we can process each message to see if it qualifies as a command
        _client.MessageReceived += HandleCommandAsync;
        // Hook CommandExecuted to handle post-execution logic
        _commands.CommandExecuted += OnCommandExecutedAsync;

        return Task.CompletedTask;
    }

    /// <summary>Processes messages to determine if they are commands and executes them if they are.</summary>
    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        if (messageParam is not SocketUserMessage message) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix
        // Also check if the message author is not a bot
        if (!message.HasStringPrefix(CommandPrefix, ref argPos) || message.Author.IsBot)
            return;

        // Create a Command Context for the processing of this message
        SocketCommandContext context = new(_client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: _services);
    }

    /// <summary>Handles post-execution logic for commands, such as logging and error handling.</summary>
    private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        // Handle custom command results with detailed responses
        if (result is CustomCommandResult customResult)
        {
            switch (customResult.ResultType)
            {
                case CustomCommandResult.CommandResultType.Permission:
                    await context.Channel.SendMessageAsync($"⛔ {result.ErrorReason}");
                    break;
                case CustomCommandResult.CommandResultType.RateLimit:
                    await context.Channel.SendMessageAsync($"⏰ {result.ErrorReason}");
                    break;
                case CustomCommandResult.CommandResultType.Warning:
                    await context.Channel.SendMessageAsync($"⚠️ {result.ErrorReason}");
                    break;
                case CustomCommandResult.CommandResultType.UserInput:
                    await context.Channel.SendMessageAsync($"❌ {result.ErrorReason}");
                    break;
                case CustomCommandResult.CommandResultType.APIError:
                case CustomCommandResult.CommandResultType.DatabaseError:
                    await context.Channel.SendMessageAsync($"🔧 {result.ErrorReason}");
                    _logger.LogError("Command error: {ErrorReason} Details: {@Details}",
                        result.ErrorReason,
                        customResult.Details);
                    break;
                default:
                    if (result.IsSuccess)
                        await context.Channel.SendMessageAsync($"✅ {result.ErrorReason}");
                    else
                        await context.Channel.SendMessageAsync($"❌ {result.ErrorReason}");
                    break;
            }
        }
        // Handle standard command results
        else if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
        {
            await context.Channel.SendMessageAsync($"Error: {result.ErrorReason}");
        }
    }
}