namespace CarSpecAPI.Services.OpenAIServices
{
    public interface IAIService
    {
        Task<string> GetResponseAsync(string conversationId ,string userMessage);
    }
}