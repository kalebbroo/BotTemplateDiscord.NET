namespace BotTemplate.BotCore.EventHandlers;

/// <summary>Triggers logic for events related to users.</summary>
/// <param name="client"></param>
internal class UserEvents(DiscordSocketClient client) // This is a personal preference to make a new class for each event type.
{
    private readonly DiscordSocketClient _client = client;

    /// <summary>Registers methods by subscribing to events. Now when that event is raised it will run your method.</summary>
    /// For a full list of events, <see cref="BaseSocketClient"/>
    public void RegisterHandlers()
    {
        _client.UserJoined += OnUserJoinedAsync;
    }

    /// <summary>Sends a welcome message when a user joins the guild.</summary>
    /// <param name="user">The user who joined the guild.</param>
    private async Task OnUserJoinedAsync(SocketGuildUser user)
    {
        await user.SendMessageAsync("Welcome to the server!");
    }
}
