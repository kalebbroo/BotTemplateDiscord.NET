using Discord.Commands;

namespace BotTemplate.BotCore.ContextCommands.Modules;

/// <summary>Contains general-purpose commands that any user can execute.
/// Demonstrates basic command structure and parameter handling.</summary>
public class UserModule(ILogger<UserModule> logger) : ContextCommandsCore
{
    private readonly ILogger<UserModule> _logger = logger;
    private readonly CommandService _commands;

    [Command("ping")]
    [Summary("Check the bot's latency.")]
    public async Task PingAsync()
    {
        if (!await HandleCooldownAsync(Context.User, "ping"))
            return;
        Int32 latency = Context.Client.Latency;
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
        string command = null)
    {
        if (!await HandleCooldownAsync(Context.User, "help"))
            return;
        // Get all commands the user can use and loops through them displaying the command name and summary.
        IEnumerable<CommandInfo> commands = _commands.Commands
            .Where(c => !c.Module.Name.Equals("admin", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(command))
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Available Commands")
                .WithColor(Color.Green)
                .WithDescription("Here are all the commands you can use:");
            foreach (CommandInfo cmd in commands)
            {
                embed.AddField(
                    $"!{cmd.Name}",
                    cmd.Summary ?? "No description available.",
                    inline: true);
            }
            await ReplyAsync(embed: embed.Build());
        }
        else
        {
            // Show help for specific command
            CommandInfo? cmd = commands.FirstOrDefault(c =>
                c.Name.Equals(command, StringComparison.OrdinalIgnoreCase) ||
                c.Aliases.Any(a => a.Equals(command, StringComparison.OrdinalIgnoreCase)));
            if (cmd == null)
            {
                await SendErrorAsync($"Command '{command}' not found.");
                return;
            }
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle($"Help - {cmd.Name}")
                .WithColor(Color.Blue)
                .AddField("Description", cmd.Summary ?? "No description available.")
                .AddField("Usage", $"!{cmd.Name} {string.Join(" ", cmd.Parameters.Select(p => $"<{p.Name}>"))}")
                .AddField("Aliases", cmd.Aliases.Any() ? string.Join(", ", cmd.Aliases) : "None");
            await ReplyAsync(embed: embed.Build());
        }
        _logger.LogDebug(
            "{User} requested help for {Command}",
            Context.User, command ?? "all commands");
    }
}
