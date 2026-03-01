using CaseAgent.Model.Requests;
using CaseAgent.Model.Responses;
using CaseAgent.Services.Interfaces;

namespace CaseAgent.Services;

public class ChargebackValidator : IChargebackValidator
{
    private static readonly HashSet<string> ValidCurrencyCodes = new()
    {
        "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "CNY", "SEK", "NZD",
        "MXN", "SGD", "HKD", "NOK", "KRW", "TRY", "INR", "RUB", "BRL", "ZAR"
    };

    public ValidationResult Validate(CreateFirstChargebackRequest request)
    {
        var errors = new List<string>();

        if (request.TransactionAmount <= 0)
        {
            errors.Add("TransactionAmount must be greater than 0");
        }

        if (request.TransactionAmount >= 999999.99m)
        {
            errors.Add("TransactionAmount must be less than 999999.99");
        }

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            errors.Add("Currency is required");
        }
        else if (!ValidCurrencyCodes.Contains(request.Currency.ToUpperInvariant()))
        {
            errors.Add("Currency must be a valid ISO 4217 code");
        }

        if (string.IsNullOrWhiteSpace(request.ReasonCode))
        {
            errors.Add("ReasonCode is required");
        }
        else if (request.ReasonCode.Length > 100)
        {
            errors.Add("ReasonCode must be 100 characters or less");
        }

        if (string.IsNullOrWhiteSpace(request.ReasonDescription))
        {
            errors.Add("ReasonDescription is required");
        }
        else if (request.ReasonDescription.Length < 1)
        {
            errors.Add("ReasonDescription must not be empty");
        }
        else if (request.ReasonDescription.Length > 10000)
        {
            errors.Add("ReasonDescription must be 10000 characters or less");
        }

        return errors.Any() ? ValidationResult.Invalid(errors) : ValidationResult.Valid();
    }
}
