using System.Text.Json.Serialization;

namespace CaseAgent.Model.Requests;

public class CreateFirstChargebackRequest
{
    [JsonPropertyName("transactionAmount")]
    public decimal TransactionAmount { get; set; }
    
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
    
    [JsonPropertyName("reasonCode")]
    public string? ReasonCode { get; set; }
    
    [JsonPropertyName("reasonDescription")]
    public string? ReasonDescription { get; set; }
}