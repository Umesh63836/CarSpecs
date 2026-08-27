namespace CarSpecAPI.Services
{
    public interface IAIService
    {
        Task<string> GetResponseAsync(string userMessage);
    }
}