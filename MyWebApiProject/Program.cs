using MyWebApiProject.Models;
using MyWebApiProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ChatGptService>();
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapPost("/chat", async (ChatRequest request, ChatGptService chatGptService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Results.BadRequest(new ChatResponse
            {
                Success = false,
                Error = "Question is required"
            });
        }

        var answer = await chatGptService.GetAnswerAsync(request.Question);
        
        return Results.Ok(new ChatResponse
        {
            Answer = answer,
            Success = true
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new ChatResponse
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("Chat")
.WithOpenApi();

app.MapPost("/gemini", async (ChatRequest request, GeminiService geminiService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Results.BadRequest(new ChatResponse
            {
                Success = false,
                Error = "Question is required"
            });
        }

        var answer = await geminiService.GetAnswerAsync(request.Question);
        
        return Results.Ok(new ChatResponse
        {
            Answer = answer,
            Success = true
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new ChatResponse
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("Gemini")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
