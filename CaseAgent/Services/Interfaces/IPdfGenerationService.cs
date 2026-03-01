namespace CaseAgent.Services.Interfaces;

public interface IPdfGenerationService
{
    Task<Guid> GenerateAndSavePdfAsync(string htmlContent);
}
