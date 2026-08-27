using OpenAI.Responses;

namespace CarSpecAPI.Services
{
    public class AIService : IAIService
    {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly ResponsesClient client;
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        private readonly IConfiguration configuration;

        public AIService(IConfiguration configuration)
        {
            this.configuration = configuration;

            var apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key is not configured.");

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            client = new ResponsesClient(apiKey);
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            var model = configuration["OpenAI:Model"]
                ?? "gpt-5.2";

            var response = await client.CreateResponseAsync(
                model, userMessage
            );

            return response.Value.GetOutputText();
        }
    }
}
