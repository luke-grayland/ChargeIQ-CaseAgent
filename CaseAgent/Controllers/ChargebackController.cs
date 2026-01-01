using System.Text;
using System.Text.Json;
using CaseAgent.Model.Requests;
using Microsoft.AspNetCore.Mvc;
using CaseAgent.Services.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace CaseAgent.Controllers;

[ApiController]
[Route("api/firstChargeback")]
public class ChargebackController(ChatClient chatClient, IToolsResponseHandler toolsResponseHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateFirstChargeback([FromBody] CreateFirstChargebackRequest request)
    {
        var serialisedRequest = JsonSerializer.Serialize(request);

        string prompt;
        var fileStream = new FileStream(@"/Users/lukegrayland/Projects/OpenAI-Test-API/CaseAgent/Prompts/FirstChargebackGenerationPrompt.txt", FileMode.Open, FileAccess.Read);
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            prompt = await streamReader.ReadToEndAsync();
        }
        
        List<ChatMessage> messages =
        [
            new SystemChatMessage(prompt),
            new UserChatMessage($"Generate a chargeback file in HTML format for this case: {serialisedRequest}")
        ];

        ChatCompletion result = await chatClient.CompleteChatAsync(messages);
        
        var chargebackFileHtml = result.Content[0].Text ?? string.Empty;
        
        return Ok(chargebackFileHtml);
    }
    
    [HttpPost]
    [Route("tool")]
    public async Task<IActionResult> Tool([FromBody] string message)
    {
        List<ChatMessage> messages =
        [
            new UserChatMessage("Find me the data for deployment 1234567890 and invoice 906589765")
        ];

        ChatCompletionOptions options = new()
        {
            Tools = { Tools.Tools.GetGetInvoiceDataTool, Tools.Tools.GetDeploymentInfoTool }
        };
        
        ChatCompletion result = await chatClient.CompleteChatAsync(messages, options);

        List<ChatMessage> chatMessages = toolsResponseHandler.HandleResponse(messages, result);

        string textResult = "Result: ";
        var toolMessages = chatMessages.OfType<ToolChatMessage>().ToList();

        foreach (var toolMessage in toolMessages)
        {
            textResult += toolMessage.Content[0].Text + " ";
        }
        
        return Ok(new { textResult });
    }
}
