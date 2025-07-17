using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MyWebApiProject.Services;

public class ChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public ChatGptService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentException("OpenAI API key is not configured");
        
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    public async Task<string> GetAnswerAsync(string question)
    {
        var requestBody = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = "당신은 친절한 AI 어시스턴트입니다. 모든 답변을 한국어로 해주세요." },
                new { role = "user", content = question }
            }
        };
        
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"OpenAI API request failed with status: {response.StatusCode}");
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseContent);
        
        var messageContent = jsonDocument.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
            
        return messageContent ?? "No response from OpenAI";
    }
}