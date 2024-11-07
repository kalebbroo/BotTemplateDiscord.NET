using Discord.Commands;

namespace BotTemplate.BotCore.TextCommands.Modules;

/// <summary>Contains general-purpose commands that any user can execute.
/// Demonstrates basic command structure and parameter handling.</summary>
public class UserModule(ILogger<UserModule> logger, CommandService command) : TextCommandsCore
{
    private readonly ILogger<UserModule> _logger = logger;
    private readonly CommandService _commands = command;

    [Command("ping")]
    [Summary("Check the bot's latency.")]
    public async Task PingAsync()
    {
        if (!await HandleCooldownAsync(Context.User, "ping"))
            return;
        int latency = Context.Client.Latency;
        await ReplyAsync($"🏓 Pong! Latency: {latency}ms");
        _logger.LogDebug("{User} checked latency: {Latency}ms", Context.User, latency);
    }

    [Command("userinfo")]
    [Summary("Get information about a user.")]
    [Alias("user", "whois")]  // Alternative command names
    public async Task UserInfoAsync(
        [Summary("The user to get info about")]
        IUser user = null)
    {
        if (!await HandleCooldownAsync(Context.User, "userinfo"))
            return;
        // If no user is specified, use the command user
        user ??= Context.User;
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle($"User Info - {user.Username}")
            .WithColor(Color.Blue)
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .AddField("User ID", user.Id, true)
            .AddField("Created At", user.CreatedAt.ToString("MM/dd/yyyy"), true);
        // Add guild-specific info if in a guild
        if (Context.Guild != null && user is IGuildUser guildUser)
        {
            embed.AddField("Joined Server", guildUser.JoinedAt?.ToString("MM/dd/yyyy") ?? "Unknown", true)
                 .AddField("Roles", guildUser.RoleIds.Count, true);
        }
        await ReplyAsync(embed: embed.Build());
        _logger.LogDebug(
            "{User} requested info about {TargetUser}",
            Context.User, user);
    }

    [Command("help")]
    [Summary("Shows a list of available commands or info about a specific command.")]
    public async Task HelpAsync(
    [Summary("The command to get help for")]
    string textCommand = null)
    {
        if (!await HandleCooldownAsync(Context.User, "help"))
            return;
        if (string.IsNullOrEmpty(textCommand))
        {
            // Show all commands
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Available Commands")
                .WithColor(Color.Green);
            StringBuilder description = new("Here are all the commands you can use:\n\n");
            foreach (CommandInfo commandInfo in _commands.Commands)
            {
                // Skip admin commands
                if (commandInfo.Module.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    continue;
                description.AppendLine($"`!{commandInfo.Name}` - {commandInfo.Summary ?? "No description available."}");
            }
            embed.Description = description.ToString();
            await ReplyAsync(embed: embed.Build());
        }
        else
        {
            // Show help for specific command
            CommandInfo? foundCommand = null;
            foreach (CommandInfo commandInfo in _commands.Commands)
            {
                if (commandInfo.Module.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (commandInfo.Name.Equals(textCommand, StringComparison.OrdinalIgnoreCase) ||
                    commandInfo.Aliases.Any(a => a.Equals(textCommand, StringComparison.OrdinalIgnoreCase)))
                {
                    foundCommand = commandInfo;
                    break;
                }
            }
            if (foundCommand == null)
            {
                await SendErrorAsync($"Command '{textCommand}' not found.");
                return;
            }
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"Help - {foundCommand.Name}")
                .WithColor(Color.Blue)
                .AddField("Description", foundCommand.Summary ?? "No description available.")
                .AddField("Usage", $"!{foundCommand.Name} {string.Join(" ", foundCommand.Parameters.Select(p => $"<{p.Name}>"))}")
                .AddField("Aliases", foundCommand.Aliases.Any() ? string.Join(", ", foundCommand.Aliases) : "None");
            await ReplyAsync(embed: embed.Build());
        }
        _logger.LogDebug(
            "{User} requested help for {Command}",
            Context.User, textCommand ?? "all commands");
    }
}
