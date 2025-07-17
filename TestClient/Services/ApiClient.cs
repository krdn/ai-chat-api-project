using System.Text;
using System.Text.Json;
using TestClient.Models;

namespace TestClient.Services;

/// <summary>
/// AI Chat API 서버와 통신하는 클라이언트 클래스
/// ChatGPT와 Gemini API 엔드포인트를 호출하여 답변을 받아옵니다.
/// </summary>
public class ApiClient
{
    #region Private Fields
    
    /// <summary>
    /// HTTP 요청을 처리하는 클라이언트 인스턴스
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// API 서버의 기본 URL
    /// </summary>
    private readonly string _baseUrl;
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// ApiClient 생성자
    /// </summary>
    /// <param name="baseUrl">API 서버의 기본 URL (기본값: http://localhost:5024)</param>
    public ApiClient(string baseUrl = "http://localhost:5024")
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// ChatGPT API 엔드포인트를 호출하여 답변을 받아옵니다.
    /// </summary>
    /// <param name="question">사용자 질문</param>
    /// <returns>ChatGPT의 답변을 포함한 응답 객체</returns>
    public async Task<ChatResponse> CallChatGptAsync(string question)
    {
        return await CallApiAsync("/chat", question);
    }

    /// <summary>
    /// Google Gemini API 엔드포인트를 호출하여 답변을 받아옵니다.
    /// </summary>
    /// <param name="question">사용자 질문</param>
    /// <returns>Gemini의 답변을 포함한 응답 객체</returns>
    public async Task<ChatResponse> CallGeminiAsync(string question)
    {
        return await CallApiAsync("/gemini", question);
    }

    /// <summary>
    /// HttpClient 리소스를 해제합니다.
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// 지정된 API 엔드포인트에 질문을 전송하고 답변을 받아오는 공통 메서드
    /// </summary>
    /// <param name="endpoint">API 엔드포인트 경로 (예: "/chat", "/gemini")</param>
    /// <param name="question">사용자 질문</param>
    /// <returns>API 서버의 응답 객체</returns>
    private async Task<ChatResponse> CallApiAsync(string endpoint, string question)
    {
        try
        {
            // 요청 객체 생성
            var request = new ChatRequest { Question = question };
            
            // JSON 직렬화
            var json = JsonSerializer.Serialize(request);
            
            // HTTP 요청 콘텐츠 생성
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API 서버에 POST 요청 전송
            var response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
            
            // 응답 내용 읽기
            var responseContent = await response.Content.ReadAsStringAsync();

            // HTTP 상태 코드 확인
            if (response.IsSuccessStatusCode)
            {
                // 성공적인 응답 처리: JSON 역직렬화
                var result = JsonSerializer.Deserialize<ChatResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true  // 대소문자 구분 없이 속성 매핑
                });
                
                // 역직렬화 결과 검증
                return result ?? new ChatResponse { Success = false, Error = "Failed to parse response" };
            }
            else
            {
                // HTTP 오류 응답 처리
                return new ChatResponse
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}: {responseContent}"
                };
            }
        }
        catch (Exception ex)
        {
            // 예외 발생 시 오류 응답 생성
            return new ChatResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
    
    #endregion
}