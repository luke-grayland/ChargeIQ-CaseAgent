namespace CaseAgent.Services.Interfaces;

public interface IPdfGenerationService
{
    Task<byte[]> GeneratePdfAsync(string htmlContent);
}
