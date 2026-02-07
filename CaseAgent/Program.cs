using CaseAgent.Middleware;
using CaseAgent.Services;
using CaseAgent.Services.Interfaces;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSingleton<ChatClient>(serviceProvider =>
{
    var apiKey = builder.Configuration["OPENAI_API_KEY"];
    var model = builder.Configuration["MODEL"];

    return new ChatClient(model, apiKey);
});

builder.Services.AddSingleton<IToolsResponseHandler, ToolsResponseHandler>();
builder.Services.AddSingleton<IPromptLoaderService, PromptLoaderService>();
builder.Services.AddSingleton<IChargebackValidator, ChargebackValidator>();
builder.Services.AddSingleton<IPdfGenerationService, PdfGenerationService>();
builder.Services.AddSingleton<IChargebackGenerationService, ChargebackGenerationService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
