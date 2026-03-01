namespace CaseAgent.Services.Interfaces;

public interface IPromptLoaderService
{
    Task<string> LoadPromptAsync(string promptFileName);
}
