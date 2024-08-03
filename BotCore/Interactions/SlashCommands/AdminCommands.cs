namespace BotTemplate.BotCore.Interactions.SlashCommands
{
    public class AdminCommands : InteractionsCore
    {
        [SlashCommand("purge", "Purges messages.")]
        public async Task PurgeAsync()
        {
            await HandleCooldown(Context.User, "purge");
            await RespondAsync($"Purged amount messages.");
        }
    }
}
