using Discord.Interactions;

namespace BotTemplate.BotCore.Interactions.SlashCommands;

[RequireUserPermission(GuildPermission.Administrator)]  // Apply to whole class for admin commands
public class AdminCommands(ILogger<AdminCommands> logger) : InteractionsCore
{
    private readonly ILogger<AdminCommands> _logger = logger;

    [SlashCommand("purge", "Deletes a specified number of messages from the channel.")]
    [RequireContext(ContextType.Guild)]  // This command only works in guild channels
    public async Task PurgeAsync(
        [Summary("amount", "The number of messages to delete (1-100)")]
        [MinValue(1)]
        [MaxValue(100)]
        int count = 10)
    {
        // Defer the response since message deletion might take a moment
        await DeferAsync(ephemeral: true);

        try
        {
            // Get messages and delete them
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            _logger.LogInformation(
                "{User} purged {Count} messages in {Channel}/{Guild}",
                Context.User, count, Context.Channel, Context.Guild);
            await FollowupAsync($"✅ Deleted {messages.Count()} messages.", ephemeral: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging messages");
            await FollowupAsync("❌ Failed to delete messages. They might be too old.", ephemeral: true);
        }
    }
}