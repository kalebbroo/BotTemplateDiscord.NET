using Discord.Interactions;
using static BotTemplate.BotCore.Interactions.Modals.Modals;

namespace BotTemplate.BotCore.Interactions.SlashCommands
{
    public class UserCommands(ILogger<UserCommands> logger) : InteractionsCore
    {
        private readonly ILogger<UserCommands> _logger = logger;

        [SlashCommand("ping", "Check the bot's latency.")]
        public async Task PingAsync()
        {
            // TODO:Add Buttons to the response to allow the user to interact with the message.
            await HandleCooldown(Context.User, "ping");
            await RespondAsync("Pong!");
        }

        [SlashCommand("echo", "Echoes the input.")]
        public async Task EchoAsync()
        {
            await HandleCooldown(Context.User, "echo");
            // When the user interacts with this command the bot responds with a Modal asking for text to echo.
            await Context.Interaction.RespondWithModalAsync<EchoConfirmModal>("echo_confirm");
        }

        [SlashCommand("test", "Test the Slash Command AutoComplete")]
        public async Task UserInfoAsync([Autocomplete(typeof(AutoComplete.AutoComplete))]
        [Summary("test", "Sends test to generate the AutoComplete")] string source)
        {
            // Defer the interaction to prevent the interaction from timing out. This is considered a response so any
            // further responses will need to be "followups" to this interaction.
            await DeferAsync(ephemeral: true);
            _logger.LogInformation("Testing AutoComplete in: slashCommands...");
            // To show an example of using the AutoComplete results, we will add each of them as an option in a select menu.
            List<SelectMenuOptionBuilder> selectMenuOptions =
            [
                new SelectMenuOptionBuilder()
                    .WithLabel(source)
                    .WithValue(source)
                    .WithDescription(source)
            ];
            // To make a select menu with the options we created above, we need to create a SelectMenuBuilder.
            // Then, we use the ComponentBuilder to turn the SelectMenuBuilder into a component. Then, we can send it.
            SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select an option")
                .WithCustomId("menu:autocomplete")
                .WithMinValues(1)
                .WithMaxValues(1)
                .WithOptions(selectMenuOptions);
            ComponentBuilder selectMenu = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);
            // Remember, this is a followup because we deferred.
            await FollowupAsync("Select an option:", components: selectMenu.Build(), ephemeral: true);
        }
    }
}
