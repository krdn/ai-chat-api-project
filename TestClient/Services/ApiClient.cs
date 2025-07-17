using System.Text;
using System.Text.Json;
using TestClient.Models;

namespace TestClient.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiClient(string baseUrl = "http://localhost:5024")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }

    public async Task<ChatResponse> CallChatGptAsync(string question)
    {
        return await CallApiAsync("/chat", question);
    }

    public async Task<ChatResponse> CallGeminiAsync(string question)
    {
        return await CallApiAsync("/gemini", question);
    }

    private async Task<ChatResponse> CallApiAsync(string endpoint, string question)
    {
        try
        {
            var request = new ChatRequest { Question = question };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<ChatResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result ?? new ChatResponse { Success = false, Error = "Failed to parse response" };
            }
            else
            {
                return new ChatResponse
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}: {responseContent}"
                };
            }
        }
        catch (Exception ex)
        {
            return new ChatResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}