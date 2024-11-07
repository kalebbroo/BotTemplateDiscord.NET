using Discord.Commands;

namespace BotTemplate.BotCore.TextCommands.Modules;

/// <summary>Contains administrative commands that require elevated permissions.
/// Demonstrates how to use permission requirements and command groups.</summary>
[Group("admin")]  // All commands in this module will be prefixed with "admin"
[RequireUserPermission(GuildPermission.Administrator)]  // Requires administrator permission
public class AdminModule(ILogger<AdminModule> logger) : TextCommandsCore
{
    private readonly ILogger<AdminModule> _logger = logger;

    [Command("purge")]
    [Summary("Deletes a specified number of messages from the channel.")]
    [RequireContext(ContextType.Guild)]  // This command only works in guild channels
    public async Task PurgeAsync(
        [Summary("The number of messages to delete (1-100)")]
        int count = 10)
    {
        // Check cooldown
        if (!await HandleCooldownAsync(Context.User, "purge"))
            return;
        // Validate input
        if (count is < 1 or > 100)
        {
            await SendErrorAsync("Please provide a number between 1 and 100.");
            return;
        }
        try
        {
            // Get messages and delete them
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(count).FlattenAsync();
            await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
            _logger.LogInformation(
                "{User} purged {Count} messages in {Channel}/{Guild}",
                Context.User, count, Context.Channel, Context.Guild);
            // Send confirmation and delete it after 5 seconds
            IUserMessage response = await ReplyAsync($"✅ Deleted {messages.Count()} messages.");
            await Task.Delay(5000);
            await response.DeleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging messages");
            await SendErrorAsync("Failed to delete messages. They might be too old.");
        }
    }

    [Command("say")]
    [Summary("Makes the bot say something in a specified channel.")]
    [RequireContext(ContextType.Guild)]
    public async Task SayAsync(
        [Summary("The channel to send the message in")]
        ITextChannel channel,
        [Summary("The message to send")][Remainder]
        string message)
    {
        if (!await HandleCooldownAsync(Context.User, "say"))
            return;
        try
        {
            await channel.SendMessageAsync(message);
            await SendSuccessAsync("Message sent!");
            _logger.LogInformation(
                "{User} used say command in {Channel}/{Guild}",
                Context.User, channel, Context.Guild);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            await SendErrorAsync("Failed to send the message. Check my permissions.");
        }
    }
}
