using CaseAgent.Model.Requests;
using CaseAgent.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using CaseAgent.Services.Interfaces;
using OpenAI.Chat;

namespace CaseAgent.Controllers;

[ApiController]
[Route("api/firstChargeback")]
public class ChargebackController : ControllerBase
{
    private readonly ChatClient _chatClient;
    private readonly IToolsResponseHandler _toolsResponseHandler;
    private readonly IChargebackValidator _validator;
    private readonly IChargebackGenerationService _generationService;

    public ChargebackController(
        ChatClient chatClient,
        IToolsResponseHandler toolsResponseHandler,
        IChargebackValidator validator,
        IChargebackGenerationService generationService)
    {
        _chatClient = chatClient;
        _toolsResponseHandler = toolsResponseHandler;
        _validator = validator;
        _generationService = generationService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChargebackGenerationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateFirstChargeback([FromBody] CreateFirstChargebackRequest request)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "Validation failed",
                Errors = validationResult.Errors,
                Timestamp = DateTime.UtcNow
            };
            return BadRequest(errorResponse);
        }

        var response = await _generationService.GenerateChargebackAsync(request);
        return Ok(response);
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

        ChatCompletion result = await _chatClient.CompleteChatAsync(messages, options);

        List<ChatMessage> chatMessages = _toolsResponseHandler.HandleResponse(messages, result);

        string textResult = "Result: ";
        var toolMessages = chatMessages.OfType<ToolChatMessage>().ToList();

        foreach (var toolMessage in toolMessages)
        {
            textResult += toolMessage.Content[0].Text + " ";
        }

        return Ok(new { textResult });
    }
}
