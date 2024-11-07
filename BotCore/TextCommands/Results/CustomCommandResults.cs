using Discord.Commands;

namespace BotTemplate.BotCore.TextCommands.Results;

// This is an optional class that can be used to create custom command results. Why use this? To provide superior error handling and more detailed command responses.
// You dont need this and the use is a personal preference. You can remove this file if you dont want to use it.

/// <summary>Represents a custom result type for command execution that provides more detailed information
/// about the outcome of a command. This allows for more granular control over command responses
/// and better error handling.</summary>
public class CustomCommandResult : RuntimeResult
{
    /// <summary>Gets whether the command execution was a success but requires additional context
    /// (e.g., partial success, warning conditions, etc.)</summary>
    public bool IsPartialSuccess;

    /// <summary>Gets additional details about the command execution that might be useful for logging
    /// or debugging purposes.</summary>
    public Dictionary<string, string> Details;

    /// <summary>Gets the type of result for more specific handling in command responses.</summary>
    public CommandResultType ResultType;

    /// <summary>Private constructor to enforce usage of static factory methods.
    /// This ensures consistent result creation across the application.</summary>
    private CustomCommandResult(
        CommandError? error,
        string reason,
        bool isPartialSuccess = false,
        Dictionary<string, string> details = null,
        CommandResultType resultType = CommandResultType.Default)
        : base(error, reason)
    {
        IsPartialSuccess = isPartialSuccess;
        Details = details ?? [];
        ResultType = resultType;
    }

    /// <summary>Different types of command results for more specific handling.
    /// Extend this enum based on your bot's needs.</summary>
    public enum CommandResultType
    {
        Default,
        Permission,
        RateLimit,
        UserInput,
        APIError,
        DatabaseError,
        Warning
    }

    #region Success Results

    /// <summary>
    /// Creates a successful result with a simple message.
    /// </summary>
    /// <example>
    /// <code>
    /// return CustomCommandResult.FromSuccess("Profile updated successfully!");
    /// </code>
    /// </example>
    public static CustomCommandResult FromSuccess(string reason = null) =>
        new(null, reason);

    /// <summary>
    /// Creates a successful result with additional details for logging or response customization.
    /// </summary>
    /// <example>
    /// <code>
    /// return CustomCommandResult.FromSuccessWithDetails(
    ///     "Items purchased successfully",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "TransactionId", "12345" },
    ///         { "ItemCount", "3" },
    ///         { "TotalCost", "150" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromSuccessWithDetails(
        string reason,
        Dictionary<string, string> details) =>
        new(null, reason, false, details);

    /// <summary>
    /// Creates a partial success result for cases where the command succeeded but with caveats.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: A bulk delete command that only deleted some messages
    /// return CustomCommandResult.FromPartialSuccess(
    ///     "Deleted 5 out of 10 messages. Some messages were too old.",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "Attempted", "10" },
    ///         { "Succeeded", "5" },
    ///         { "Failed", "5" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromPartialSuccess(
        string reason,
        Dictionary<string, string> details = null) =>
        new(null, reason, true, details, CommandResultType.Warning);

    #endregion

    #region Error Results

    /// <summary>
    /// Creates an error result for permission-related failures.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: User tried to use an admin command without proper permissions
    /// return CustomCommandResult.FromPermissionError(
    ///     "You need Administrator permission to use this command",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "RequiredPermission", "Administrator" },
    ///         { "UserPermissions", "Moderator, User" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromPermissionError(
        string reason,
        Dictionary<string, string> details = null) =>
        new(CommandError.UnmetPrecondition, reason, false, details, CommandResultType.Permission);

    /// <summary>
    /// Creates an error result for rate limit or cooldown violations.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: User tried to use a command too quickly
    /// return CustomCommandResult.FromRateLimit(
    ///     "Please wait 5 seconds before using this command again",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "RemainingSeconds", "5" },
    ///         { "CommandName", "daily" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromRateLimit(
        string reason,
        Dictionary<string, string> details = null) =>
        new(CommandError.UnmetPrecondition, reason, false, details, CommandResultType.RateLimit);

    /// <summary>
    /// Creates an error result for invalid user input.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: User provided an invalid number for a bet
    /// return CustomCommandResult.FromUserError(
    ///     "Bet amount must be between 10 and 1000 coins",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "ProvidedAmount", "5000" },
    ///         { "MinAmount", "10" },
    ///         { "MaxAmount", "1000" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromUserError(
        string reason,
        Dictionary<string, string> details = null) =>
        new(CommandError.ParseFailed, reason, false, details, CommandResultType.UserInput);

    /// <summary>
    /// Creates an error result for API-related failures.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example: External API call failed
    /// return CustomCommandResult.FromAPIError(
    ///     "Unable to fetch weather data at this time",
    ///     new Dictionary<string, string>
    ///     {
    ///         { "StatusCode", "429" },
    ///         { "APIEndpoint", "weather/current" },
    ///         { "RetryAfter", "60" }
    ///     });
    /// </code>
    /// </example>
    public static CustomCommandResult FromAPIError(
        string reason,
        Dictionary<string, string> details = null) =>
        new(CommandError.Exception, reason, false, details, CommandResultType.APIError);

    #endregion

    #region Usage Examples

    /*
    // Example 1: Using in a command with simple success
    [Command("ping")]
    public Task<RuntimeResult> PingAsync()
    {
        var latency = Context.Client.Latency;
        return Task.FromResult<RuntimeResult>(
            CustomCommandResult.FromSuccess($"Pong! Latency: {latency}ms"));
    }

    // Example 2: Using in a command with detailed error handling
    [Command("buy")]
    public async Task<RuntimeResult> BuyItemAsync(string itemName, int quantity)
    {
        var userBalance = await _economy.GetBalanceAsync(Context.User.Id);
        var item = await _itemService.GetItemAsync(itemName);

        if (item == null)
            return CustomCommandResult.FromUserError(
                "Item not found",
                new Dictionary<string, string>
                {
                    { "RequestedItem", itemName },
                    { "AvailableItems", "apple,banana,orange" }
                });

        var totalCost = item.Price * quantity;
        if (userBalance < totalCost)
            return CustomCommandResult.FromUserError(
                "Insufficient funds",
                new Dictionary<string, string>
                {
                    { "UserBalance", userBalance.ToString() },
                    { "RequiredAmount", totalCost.ToString() },
                    { "Missing", (totalCost - userBalance).ToString() }
                });

        // Process purchase...
        return CustomCommandResult.FromSuccessWithDetails(
            $"Successfully purchased {quantity}x {itemName}!",
            new Dictionary<string, string>
            {
                { "ItemId", item.Id.ToString() },
                { "Quantity", quantity.ToString() },
                { "TotalCost", totalCost.ToString() },
                { "NewBalance", (userBalance - totalCost).ToString() }
            });
    }
    */

    #endregion
}