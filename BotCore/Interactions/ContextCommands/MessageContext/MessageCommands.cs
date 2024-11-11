using Discord.Interactions;

namespace BotTemplate.BotCore.Interactions.ContextCommands;

public class MessageCommands(ILogger<MessageCommands> logger) : InteractionsCore
{
    private readonly ILogger<MessageCommands> _logger = logger;

    [MessageCommand("Uwuify Message")] // Appears when right-clicking a message
    public async Task UwuifyMessageAsync(IMessage message)
    {
        string uwuified = message.Content
            .Replace('r', 'w')
            .Replace('l', 'w')
            .Replace("na", "nya")
            .Replace("the", "da");

        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("UwU-ified Message! 🌸")
            .WithDescription(uwuified)
            .WithColor(Color.Magenta)
            .WithFooter($"Transformed by {Context.User.Username}")
            .WithCurrentTimestamp();

        await RespondAsync(embed: embed.Build());
    }
}