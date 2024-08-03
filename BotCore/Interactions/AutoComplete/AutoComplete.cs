namespace BotTemplate.BotCore.Interactions.AutoComplete
{
    public class AutoComplete : AutocompleteHandler
    {
        /// <summary>Generates autocomplete suggestions for the specified parameter.</summary>
        /// <param name="context"></param>
        /// <param name="autocompleteInteraction"></param>
        /// <param name="parameter"></param>
        /// <param name="services"></param>
        /// <returns>a list of autocomplete results</returns>
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction,
            IParameterInfo parameter, IServiceProvider services)
        {
            IEnumerable<AutocompleteResult> results = [];
            List<string> options = ["Option1", "Option2", "Option3", "Option4", "Option5"];

            // You can add more cases for each autocomplete you want to generate. Look at Slash Commands for the "test" example.
            switch (parameter.Name)
            {
                case "test":
                    results = options
                        .Select(option => new AutocompleteResult(option, option))
                        .Take(25)
                        .ToList();
                    break;
                default:
                    break;
            }
            // Return the results, limited to 25 because Discord is dumb.
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
