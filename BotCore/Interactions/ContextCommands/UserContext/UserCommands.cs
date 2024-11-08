namespace BotTemplate.BotCore.Interactions.ContextCommands.UserContext;

/// <summary>The Boop command lets users playfully boop each other.
/// Appears when right-clicking a user and creates a fun message.</summary>
public class BoopCommand : IUserContextCommand
{
    public string CommandName => "Boop!";

    public async Task<string> HandleAsync(SocketUserCommand command)
    {
        string mentionedUser = command.Data.Member.Mention;
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("Boop! 👉👃")
            .WithDescription($"{command.User.Mention} booped {mentionedUser}!")
            .WithColor(Color.Purple)
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embed.Build());
        return null;
    }
}

/// <summary>The Profile command shows detailed information about a user.
/// Creates a card showing their join date, avatar, and other details.</summary>
public class ProfileCommand : IUserContextCommand
{
    public string CommandName => "View Profile Card";

    public async Task<string> HandleAsync(SocketUserCommand command)
    {
        SocketUser targetUser = command.Data.Member;
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle($"Profile Card - {targetUser.Username}")
            .WithThumbnailUrl(targetUser.GetAvatarUrl() ?? targetUser.GetDefaultAvatarUrl())
            .WithColor(Color.Blue)
            .AddField("User ID", targetUser.Id, true)
            .AddField("Created Account", targetUser.CreatedAt.ToString("MMM dd, yyyy"), true)
            .AddField("Bot Account", targetUser.IsBot ? "Yes" : "No", true);
        if (targetUser is SocketGuildUser guildUser)
        {
            embed.AddField("Joined Server",
                    guildUser.JoinedAt?.ToString("MMM dd, yyyy") ?? "Unknown", true)
                .AddField("Nickname",
                    string.IsNullOrEmpty(guildUser.Nickname) ? "None" : guildUser.Nickname, true)
                .AddField("Top Role",
                    guildUser.Roles.Max()?.Name ?? "No Roles", true);
        }
        embed.WithFooter($"Requested by {command.User.Username}")
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embed.Build());
        return null;
    }
}
