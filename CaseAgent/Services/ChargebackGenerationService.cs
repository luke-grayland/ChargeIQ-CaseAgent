using System.Text.Json;
using CaseAgent.Model.Requests;
using CaseAgent.Services.Interfaces;
using OpenAI.Chat;

namespace CaseAgent.Services;

public class ChargebackGenerationService(IPromptLoaderService promptLoader, ChatClient chatClient,
    IPdfGenerationService pdfGenerator) : IChargebackGenerationService
{
    public async Task<byte[]> GenerateChargebackAsync(CreateFirstChargebackRequest request)
    {
        var prompt = await promptLoader.LoadPromptAsync("FirstChargebackGenerationPrompt.txt");
        var serializedRequest = JsonSerializer.Serialize(request);

        List<ChatMessage> messages =
        [
            new SystemChatMessage(prompt),
            new UserChatMessage($"Generate a chargeback file in HTML format for this case: {serializedRequest}")
        ];

        ChatCompletion result = await chatClient.CompleteChatAsync(messages);
        var htmlContent = result.Content[0].Text ?? string.Empty;

        return await pdfGenerator.GeneratePdfAsync(htmlContent);
    }
}
