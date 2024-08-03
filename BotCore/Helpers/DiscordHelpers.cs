namespace BotTemplate.BotCore.Helpers;

public static class DiscordHelpers
{
    /// <summary>Ran when the Discord client is ready. This method logs information about the bot, registers commands globally, and sets the bot's status.
    /// Add or edit the logic here to run the moment the bot is online and read.</summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    internal static async Task ClientReady(IServiceProvider serviceProvider)
    {
        ILogger<Program> logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            InteractionService interactions = serviceProvider.GetRequiredService<InteractionService>();
            // In this section you need to choose whether to register commands globally or per guild. I normally register them per guild during development.
            // To register commands globally, move RegisterCommandsGloballyAsync() after AddModulesAsync() and remove the for loop.
            await interactions!.RegisterCommandsGloballyAsync(true); // Running it before AddModulesAsync() will clear all global commands // DEBUG ONLY
            if (client!.Guilds.Count != 0)
            {
                // Scans the whole assembly for classes that define slash commands and registers the command modules with the InteractionService.
                await interactions!.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);
                foreach (SocketGuild? guild in client.Guilds)
                {
                    await interactions.RegisterCommandsToGuildAsync(guild.Id, true);
                }
            }
            else
            {
                logger?.LogWarning("No guilds found");
            }
            logger?.LogInformation("Logged in as {Username}", client.CurrentUser.Username);
            logger?.LogInformation("Registered {Count} slash commands", interactions!.SlashCommands.Count);
            logger?.LogInformation("Bot is a member of {Count} guilds", client.Guilds.Count);
            await client.SetGameAsync("/help", null, ActivityType.Listening);
        }
        catch (Exception ex)
        {
            logger.LogError("{Exception}", ex.Message);
            throw;
        }
    }

    /// <summary>Logs a message to the console and Serilog.</summary>
    /// <param name="message">The LogMessage object to log.</param>
    internal static async Task LogMessageAsync(LogMessage message)
    {
        LogEventLevel severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }
}
