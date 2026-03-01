namespace CaseAgent.Model.Responses;

public class ChargebackGenerationResponse
{
    public Guid FileId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public bool Success { get; set; }
}
