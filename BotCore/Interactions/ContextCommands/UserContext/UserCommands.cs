using Discord.Interactions;

namespace BotTemplate.BotCore.Interactions.ContextCommands;

public class UserCommands(ILogger<UserCommands> logger) : InteractionsCore
{
    private readonly ILogger<UserCommands> _logger = logger;

    [UserCommand("Boop!")] // Appears when right-clicking a user
    public async Task BoopUserAsync(IUser user)
    {
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("Boop! 👉👃")
            .WithDescription($"{Context.User.Mention} booped {user.Mention}!")
            .WithColor(Color.Purple)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embed.Build());
    }
}