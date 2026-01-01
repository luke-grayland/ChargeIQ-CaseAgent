using OpenAI.Chat;

namespace CaseAgent.Services.Interfaces;

public interface IToolsResponseHandler
{
    List<ChatMessage> HandleResponse(List<ChatMessage> messages, ChatCompletion chatCompletion);
}