using CaseAgent.Model.Requests;

namespace CaseAgent.Services.Interfaces;

public interface IChargebackGenerationService
{
    Task<byte[]> GenerateChargebackAsync(CreateFirstChargebackRequest request);
}
