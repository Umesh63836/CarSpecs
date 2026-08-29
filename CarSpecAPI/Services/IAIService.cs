namespace CarSpecAPI.Services
{
    public interface IAIService
    {
        Task<string> GetResponseAsync(string conversationId ,string userMessage);
    }
}