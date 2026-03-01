using CaseAgent.Services.Interfaces;
using PuppeteerSharp;

namespace CaseAgent.Services;

public class PdfGenerationService : IPdfGenerationService
{
    private readonly string _outputDirectory;
    private readonly ILogger<PdfGenerationService> _logger;

    public PdfGenerationService(ILogger<PdfGenerationService> logger)
    {
        _logger = logger;
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        _outputDirectory = Path.Combine(desktopPath, "outputs");

        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
            _logger.LogInformation("Created output directory: {OutputDirectory}", _outputDirectory);
        }
    }

    public async Task<Guid> GenerateAndSavePdfAsync(string htmlContent)
    {
        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}.pdf";
        var filePath = Path.Combine(_outputDirectory, fileName);

        try
        {
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);

            await page.PdfAsync(filePath, new PdfOptions
            {
                Format = PuppeteerSharp.Media.PaperFormat.A4,
                PrintBackground = true
            });

            _logger.LogInformation("Generated PDF: {FilePath}", filePath);
            return fileId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate PDF");
            throw new InvalidOperationException("Failed to generate PDF", ex);
        }
    }
}
