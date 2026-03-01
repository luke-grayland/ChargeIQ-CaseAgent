using CaseAgent.Model.Requests;
using CaseAgent.Model.Responses;

namespace CaseAgent.Services.Interfaces;

public interface IChargebackGenerationService
{
    Task<ChargebackGenerationResponse> GenerateChargebackAsync(CreateFirstChargebackRequest request);
}
