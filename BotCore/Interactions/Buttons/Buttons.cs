namespace BotTemplate.BotCore.Interactions.Buttons
{
    public class Buttons : InteractionsCore
    {
        [ComponentInteraction("button:*", runMode: RunMode.Async)]
        public async Task ButtonInteraction(string customId)
        {
            string userId = Context.User.Id.ToString();
            switch (userId)
            {
                case string id when id == customId:
                    await FollowupAsync("You clicked the button.");
                    break;
                default:
                    await FollowupAsync("You are not allowed to interact with this button.", ephemeral: true);
                    break;
            }
        }
    }
}
