namespace BotTemplate.BotCore.Interactions.ContextCommands;

/// <summary>This interface marks a class as a context command and provides the name that will appear
/// in Discord's right-click menu. Any class implementing this will automatically be registered
/// as a context command.</summary>
public interface IContextCommand
{
    /// <summary>The name that appears in Discord's right-click menu</summary>
    string CommandName { get; }
}

/// <summary>Marks a command that appears when right-clicking a user</summary>
public interface IUserContextCommand : IContextCommand
{
    Task<string> HandleAsync(SocketUserCommand command);
}

/// <summary>Marks a command that appears when right-clicking a message</summary>
public interface IMessageContextCommand : IContextCommand
{
    Task<string> HandleAsync(SocketMessageCommand command);
}