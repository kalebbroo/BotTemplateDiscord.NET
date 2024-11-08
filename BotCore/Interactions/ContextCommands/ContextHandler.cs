namespace BotTemplate.BotCore.Interactions.ContextCommands;

/// <summary>Handles Discord's context menu commands - these are commands that appear when you right-click
/// on a user or message. This handler automatically finds all commands and registers them with Discord.</summary>
public class ContextHandler
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<ContextHandler> _logger;

    // Store our commands in dictionaries for quick lookup
    private readonly Dictionary<string, IUserContextCommand> _userCommands = new();
    private readonly Dictionary<string, IMessageContextCommand> _messageCommands = new();

    public ContextHandler(DiscordSocketClient client, ILogger<ContextHandler> logger)
    {
        _client = client;
        _logger = logger;
        LoadCommands();
    }

    /// <summary>Finds all context commands in the assembly by looking for classes that implement
    /// IUserContextCommand or IMessageContextCommand. This means new commands are automatically
    /// found without needing to edit this file.</summary>
    private void LoadCommands()
    {
        // Get all types in our assembly that implement our command interfaces
        Assembly assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> commandTypes = assembly.GetTypes().Where(type =>
            !type.IsAbstract && !type.IsInterface &&
            (typeof(IUserContextCommand).IsAssignableFrom(type) ||
             typeof(IMessageContextCommand).IsAssignableFrom(type)));
        foreach (Type type in commandTypes)
        {
            // Create an instance of the command
            object? command = Activator.CreateInstance(type);
            if (command is IUserContextCommand userCommand)
            {
                _userCommands[userCommand.CommandName] = userCommand;
                _logger.LogInformation("Loaded user context command: {Command}", userCommand.CommandName);
            }
            else if (command is IMessageContextCommand messageCommand)
            {
                _messageCommands[messageCommand.CommandName] = messageCommand;
                _logger.LogInformation("Loaded message context command: {Command}", messageCommand.CommandName);
            }
        }
    }

    /// <summary>Registers all found commands with Discord so they appear in context menus.</summary>
    public async Task RegisterContextCommands()
    {
        try
        {
            List<ApplicationCommandProperties> commands = new();
            // Register user commands
            foreach (IUserContextCommand command in _userCommands.Values)
            {
                commands.Add(new UserCommandBuilder()
                    .WithName(command.CommandName)
                    .Build());
            }
            // Register message commands
            foreach (IMessageContextCommand command in _messageCommands.Values)
            {
                commands.Add(new MessageCommandBuilder()
                    .WithName(command.CommandName)
                    .Build());
            }
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(commands.ToArray());
            _logger.LogInformation("Registered {Count} context menu commands with Discord", commands.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register context commands");
        }
    }

    /// <summary>Handles execution of user context commands by finding the right command handler</summary>
    public async Task HandleUserCommand(SocketUserCommand command)
    {
        try
        {
            if (_userCommands.TryGetValue(command.CommandName, out IUserContextCommand? handler))
            {
                await handler.HandleAsync(command);
            }
            else
            {
                _logger.LogWarning("Unknown user command: {Command}", command.CommandName);
                await command.RespondAsync("This command isn't set up yet!", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling user command: {Command}", command.CommandName);
            await command.RespondAsync("Something went wrong while handling that command.", ephemeral: true);
        }
    }

    /// <summary>Handles execution of message context commands by finding the right command handler</summary>
    public async Task HandleMessageCommand(SocketMessageCommand command)
    {
        try
        {
            if (_messageCommands.TryGetValue(command.CommandName, out IMessageContextCommand? handler))
            {
                await handler.HandleAsync(command);
            }
            else
            {
                _logger.LogWarning("Unknown message command: {Command}", command.CommandName);
                await command.RespondAsync("This command isn't set up yet!", ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message command: {Command}", command.CommandName);
            await command.RespondAsync("Something went wrong while handling that command.", ephemeral: true);
        }
    }
}