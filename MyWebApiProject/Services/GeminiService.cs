using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MyWebApiProject.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    
    public GeminiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini API key is not configured");
    }
    
    public async Task<string> GetAnswerAsync(string question)
    {
        try
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = $"당신은 친절한 AI 어시스턴트입니다. 모든 답변을 한국어로 해주세요. 질문: {question}" }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 1024
                }
            };
            
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var url = $"{BaseUrl}?key={_apiKey}";
            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed with status: {response.StatusCode}. Error: {errorContent}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            
            var candidates = jsonDocument.RootElement.GetProperty("candidates");
            if (candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                var content_parts = firstCandidate.GetProperty("content").GetProperty("parts");
                if (content_parts.GetArrayLength() > 0)
                {
                    var text = content_parts[0].GetProperty("text").GetString();
                    return text ?? "No response from Gemini";
                }
            }
            
            return "No response from Gemini";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error calling Gemini API: {ex.Message}");
        }
    }
}