namespace BotTemplate.BotCore.Interactions.ContextCommands.MessageContext;

/// <summary>Remind command lets users set a reminder about a message.
/// This is a demo showing how a reminder system could work.</summary>
public class RemindCommand : IMessageContextCommand
{
    public string CommandName => "Remind Me Later";

    public async Task<string> HandleAsync(SocketMessageCommand command)
    {
        SocketMessage message = command.Data.Message;
        string messagePreview = message.Content[..Math.Min(100, message.Content.Length)];
        if (message.Content.Length > 100)
            messagePreview += "...";
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("📬 Reminder Set!")
            .WithDescription("I'll remind you about this message in 1 hour:")
            .AddField("Message Preview", messagePreview)
            .WithColor(Color.Green)
            .WithFooter($"Message ID: {message.Id}")
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embed.Build(), ephemeral: true);
        return null;
    }
}

/// <summary>Uwuify command transforms messages into cute "uwu" speak.
/// A fun command showing how to transform message content!</summary>
public class UwuifyCommand : IMessageContextCommand
{
    public string CommandName => "Uwuify Message";

    public async Task<string> HandleAsync(SocketMessageCommand command)
    {
        string uwuifiedText = command.Data.Message.Content
            .Replace('r', 'w')
            .Replace('l', 'w')
            .Replace('R', 'W')
            .Replace('L', 'W')
            .Replace("na", "nya")
            .Replace("Na", "Nya")
            .Replace("NA", "NYA")
            .Replace("the", "da")
            .Replace("The", "Da");
        string[] uwuEndings = { " uwu", " owo", " >w<", " :3", " nyaa~" };
        Random random = new Random();
        string[] sentences = uwuifiedText.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < sentences.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(sentences[i]))
                sentences[i] = sentences[i].Trim() + uwuEndings[random.Next(uwuEndings.Length)];
        }
        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle("UwU-ified Message! 🌸")
            .WithDescription(string.Join(". ", sentences))
            .WithColor(Color.Magenta)
            .WithFooter($"Transformed by {command.User.Username}")
            .WithCurrentTimestamp();
        await command.RespondAsync(embed: embed.Build());
        return null;
    }
}
