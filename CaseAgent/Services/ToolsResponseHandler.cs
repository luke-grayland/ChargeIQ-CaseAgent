using System.Text.Json;
using CaseAgent.Services.Interfaces;
using OpenAI.Chat;

namespace CaseAgent.Services;

public class ToolsResponseHandler : IToolsResponseHandler
{
    public List<ChatMessage> HandleResponse(List<ChatMessage> messages, ChatCompletion completion)
    {
        bool requiresAction;

        do
        {
            requiresAction = false;

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        // Add the assistant message to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        // First, add the assistant message with tool calls to the conversation history.
                        messages.Add(new AssistantChatMessage(completion));

                        // Then, add a new tool message for each tool call that is resolved.
                        foreach (ChatToolCall toolCall in completion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case nameof(Tools.Tools.GetInvoiceData):
                                {
                                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                    bool hasInvoiceId = argumentsJson.RootElement.TryGetProperty("invoiceId", out JsonElement invoiceIdJson);

                                    if (!hasInvoiceId)
                                    {
                                        throw new ArgumentNullException(nameof(invoiceIdJson), "The invoice ID argument is required.");
                                    }

                                    if (!int.TryParse(invoiceIdJson.ToString(), out int invoiceId))
                                    {
                                        throw new ArgumentNullException(nameof(invoiceIdJson), "The invoice ID argument is not an integer.");
                                    }

                                    string toolResult = Tools.Tools.GetInvoiceData(invoiceId); 
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }
                                
                                case nameof(Tools.Tools.GetDeploymentInfo):
                                    {
                                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                        bool hasDeploymentId = argumentsJson.RootElement.TryGetProperty("deploymentId", out JsonElement deploymentIdJson);

                                        if (!hasDeploymentId)
                                        {
                                            throw new ArgumentNullException(nameof(deploymentIdJson), "The deployment ID argument is required.");
                                        }

                                        if (!int.TryParse(deploymentIdJson.ToString(), out int deploymentId))
                                        {
                                            throw new ArgumentNullException(nameof(deploymentIdJson), "The deployment ID argument is not an integer.");
                                        }

                                        string toolResult = Tools.Tools.GetDeploymentInfo(deploymentId); 
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                default:
                                    {
                                        // Handle other unexpected calls.
                                        throw new NotImplementedException();
                                    }
                            }
                        }

                        //requiresAction = true;
                        break;
                    }

                case ChatFinishReason.Length:
                    throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                case ChatFinishReason.ContentFilter:
                    throw new NotImplementedException("Omitted content due to a content filter flag.");

                case ChatFinishReason.FunctionCall:
                    throw new NotImplementedException("Deprecated in favor of tool calls.");

                default:
                    throw new NotImplementedException(completion.FinishReason.ToString());
            }
        } while (requiresAction);
        
        return messages;
    }
}