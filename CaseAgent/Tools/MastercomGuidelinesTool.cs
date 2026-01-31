using System.Text;
using OpenAI.Chat;

namespace CaseAgent.Tools;

public static class MastercomGuidelinesTool
{
    public static string GetMastercomGuidelines()
    {
        string text;
        var promptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "MastercomLLMDoc.txt");
        var fileStream = new FileStream(promptPath, FileMode.Open, FileAccess.Read);
        
        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        {
            text = streamReader.ReadToEnd();
        }

        return text;
    }
    
    public static readonly ChatTool GetGetInvoiceDataTool = ChatTool.CreateFunctionTool(
        functionName: nameof(GetMastercomGuidelines),
        functionDescription: "Get Mastercom guidelines document");
}