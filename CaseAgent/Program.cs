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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
