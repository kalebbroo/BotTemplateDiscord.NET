namespace BotTemplate.BotCore.Interactions.SelectMenus
{
    public class SelectMenus : InteractionsCore
    {
        [ComponentInteraction("menu:*", runMode: RunMode.Async)]
        public async Task DisplaySearchResults(string customId, string[] selections)
        {
            string selection = selections.FirstOrDefault() ?? throw new ArgumentNullException(nameof(selections));
            string userId = Context.User.Id.ToString();
            switch (userId)
            {
                case string id when id == customId:
                    await RespondAsync($"You selected {selection}.", ephemeral: true);
                    break;
                default:
                    await RespondAsync("You are not allowed to interact with this select menu.", ephemeral: true);
                    break;
            }
        }
    }
}
