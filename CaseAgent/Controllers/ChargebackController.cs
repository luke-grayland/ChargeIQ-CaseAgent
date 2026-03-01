using CaseAgent.Model.Requests;
using CaseAgent.Model.Responses;
using Microsoft.AspNetCore.Mvc;
using CaseAgent.Services.Interfaces;

namespace CaseAgent.Controllers;

[ApiController]
[Route("api/firstChargeback")]
public class ChargebackController(IChargebackValidator validator, IChargebackGenerationService generationService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateFirstChargeback([FromBody] CreateFirstChargebackRequest request)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorResponse = new ErrorResponse
            {
                Message = "Validation failed",
                Errors = validationResult.Errors,
                Timestamp = DateTime.UtcNow
            };
            return BadRequest(errorResponse);
        }

        var pdfBytes = await generationService.GenerateChargebackAsync(request);
        return File(pdfBytes, "application/pdf", $"first-chargeback-{Guid.NewGuid():N}.pdf");
    }
}
