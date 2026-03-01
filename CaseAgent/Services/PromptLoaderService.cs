using System.Text;
using CaseAgent.Services.Interfaces;

namespace CaseAgent.Services;

public class PromptLoaderService : IPromptLoaderService
{
    private readonly string _promptsDirectory;

    public PromptLoaderService()
    {
        _promptsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts");
    }

    public async Task<string> LoadPromptAsync(string promptFileName)
    {
        var promptPath = Path.Combine(_promptsDirectory, promptFileName);

        if (!File.Exists(promptPath))
        {
            throw new FileNotFoundException($"Prompt file not found: {promptFileName}", promptPath);
        }

        using var fileStream = new FileStream(promptPath, FileMode.Open, FileAccess.Read);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8);

        return await streamReader.ReadToEndAsync();
    }
}
