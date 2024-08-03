namespace BotTemplate.BotCore.Interactions.Modals
{
    public class Modals : InteractionsCore
    {
        /// <summary>Represents a modal for echoing user input.</summary>
        public class EchoConfirmModal : IModal
        {
            public string Title => "Confirmation";
            [InputLabel("What to Echo?")]
            [ModalTextInput("echo", TextInputStyle.Paragraph, placeholder: "Enter text to echo")]
            public string? EchoText { get; set; }
        }

        /// <summary>Handles the submission of the echo_confirm Modal. Extracts the text then responds</summary>
        [ModalInteraction("echo_confirm")]
        public async Task OnTestModalSubmit(EchoConfirmModal modal)
        {
            // Extract the submitted text from the modal
            string submittedText = modal.EchoText ?? "No text was submitted.";
            await RespondAsync($"You said: {submittedText}", ephemeral: true);
        }
    }
}