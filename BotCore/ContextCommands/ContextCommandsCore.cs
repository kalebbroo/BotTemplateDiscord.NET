using Discord.Commands;

namespace BotTemplate.BotCore.ContextCommands;

/// <summary>
/// Base class for all context command modules. Provides common functionality like cooldowns
/// and helper methods that can be used across all command modules.
/// </summary>
public class ContextCommandsCore : ModuleBase<SocketCommandContext>
{
    private static readonly Dictionary<(ulong, string), DateTime> _lastInteracted = [];
    private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(3);

    /// <summary>
    /// Checks if a user is on cooldown for a specific command.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="command">The command name</param>
    /// <returns>True if the user is on cooldown, false otherwise</returns>
    protected static bool IsOnCooldown(SocketUser user, string command)
    {
        (ulong Id, string command) key = (user.Id, command);
        if (_lastInteracted.TryGetValue(key, out DateTime lastInteraction))
        {
            if (DateTime.UtcNow - lastInteraction < Cooldown)
            {
                return true;
            }
        }
        _lastInteracted[key] = DateTime.UtcNow;
        return false;
    }

    /// <summary>
    /// Helper method to handle cooldown responses.
    /// Use this in commands to ensure consistent cooldown handling.
    /// </summary>
    /// <param name="user">The user to check</param>
    /// <param name="command">The command name</param>
    /// <returns>True if command should proceed (not on cooldown), false otherwise</returns>
    protected async Task<bool> HandleCooldownAsync(SocketUser user, string command)
    {
        if (IsOnCooldown(user, command))
        {
            await ReplyAsync("You are on cooldown for this command.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Helper method to send error messages with consistent formatting.
    /// </summary>
    protected async Task SendErrorAsync(string message)
    {
        await ReplyAsync($"❌ Error: {message}");
    }

    /// <summary>
    /// Helper method to send success messages with consistent formatting.
    /// </summary>
    protected async Task SendSuccessAsync(string message)
    {
        await ReplyAsync($"✅ {message}");
    }
}