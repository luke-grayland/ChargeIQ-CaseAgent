using CaseAgent.Model.Requests;
using CaseAgent.Model.Responses;

namespace CaseAgent.Services.Interfaces;

public interface IChargebackValidator
{
    ValidationResult Validate(CreateFirstChargebackRequest request);
}
