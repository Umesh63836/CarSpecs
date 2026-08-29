using CarSpecAPI.Data.Models.RequestModel;
using System.Collections.Concurrent;

namespace CarSpecAPI.Services
{
    public class ConversationService
    {
        private readonly ConcurrentDictionary<string, List<ChatHistoryDto>> conversations = new(); 

        public List<ChatHistoryDto> GetHistory(string conversationId) 
        { 
            return conversations.GetOrAdd(conversationId, _ => new List<ChatHistoryDto>()); 
        }

        public void AddMessage(string conversationId, string role, string content) 
        { 
            var history = conversations.GetOrAdd(conversationId, _ => new List<ChatHistoryDto>()); lock (history) 
            { 
                history.Add(new ChatHistoryDto { Role = role, Content = content }); 
            } 
        }
    }
}
