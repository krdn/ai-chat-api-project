using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MyWebApiProject.Services;

/// <summary>
/// Google Gemini API와 상호작용하는 서비스 클래스
/// Gemini 1.5 Flash 모델을 사용하여 자연어 질문에 대한 답변을 생성합니다.
/// </summary>
public class GeminiService
{
    #region Private Fields
    
    /// <summary>
    /// HTTP 요청을 처리하는 클라이언트
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// Google AI Studio에서 발급받은 API 키
    /// </summary>
    private readonly string _apiKey;
    
    /// <summary>
    /// Google Gemini API의 기본 엔드포인트 URL
    /// gemini-1.5-flash 모델을 사용하여 콘텐츠를 생성합니다.
    /// </summary>
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// GeminiService 생성자
    /// </summary>
    /// <param name="configuration">애플리케이션 설정 객체 (appsettings.json에서 API 키 읽기)</param>
    /// <param name="httpClient">DI 컨테이너에서 주입받는 HttpClient 인스턴스</param>
    /// <exception cref="ArgumentException">Gemini API 키가 설정되지 않았을 때 발생</exception>
    public GeminiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        // appsettings.json에서 "Gemini:ApiKey" 값을 읽어옵니다
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentException("Gemini API key is not configured");
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 사용자 질문에 대한 Gemini AI의 답변을 비동기적으로 가져옵니다.
    /// </summary>
    /// <param name="question">사용자의 질문 텍스트</param>
    /// <returns>Gemini AI의 답변 텍스트</returns>
    /// <exception cref="Exception">API 호출 실패 시 발생</exception>
    public async Task<string> GetAnswerAsync(string question)
    {
        try
        {
            // Gemini API 요청 본문 구성
            var requestBody = new
            {
                // 대화 내용 배열 - 시스템 프롬프트와 사용자 질문을 포함
                contents = new[]
                {
                    new
                    {
                        // 메시지 파트 배열
                        parts = new[]
                        {
                            new { 
                                text = $"당신은 친절한 AI 어시스턴트입니다. 모든 답변을 한국어로 해주세요. 질문: {question}" 
                            }
                        }
                    }
                },
                // 텍스트 생성 설정
                generationConfig = new
                {
                    temperature = 0.7,      // 창의성 조절 (0.0 ~ 1.0, 높을수록 창의적)
                    topK = 40,              // 샘플링할 토큰 수 제한
                    topP = 0.95,            // 누적 확률 임계값 (0.0 ~ 1.0)
                    maxOutputTokens = 1024  // 최대 출력 토큰 수
                }
            };
            
            // 요청 본문을 JSON 문자열로 직렬화
            var json = JsonSerializer.Serialize(requestBody);
            
            // HTTP 요청 콘텐츠 생성 (UTF-8 인코딩, application/json 타입)
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // API 키를 쿼리 파라미터로 추가한 완전한 URL 생성
            var url = $"{BaseUrl}?key={_apiKey}";
            
            // Gemini API에 POST 요청 전송
            var response = await _httpClient.PostAsync(url, content);
            
            // HTTP 응답 상태 코드 확인
            if (!response.IsSuccessStatusCode)
            {
                // 오류 응답 내용 읽기
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed with status: {response.StatusCode}. Error: {errorContent}");
            }
            
            // 성공적인 응답 내용 읽기
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // JSON 응답 파싱
            var jsonDocument = JsonDocument.Parse(responseContent);
            
            // 응답에서 후보 답변들 추출
            var candidates = jsonDocument.RootElement.GetProperty("candidates");
            
            // 첫 번째 후보 답변이 있는지 확인
            if (candidates.GetArrayLength() > 0)
            {
                var firstCandidate = candidates[0];
                
                // 답변 콘텐츠의 parts 배열 추출
                var content_parts = firstCandidate.GetProperty("content").GetProperty("parts");
                
                // 첫 번째 part에서 텍스트 추출
                if (content_parts.GetArrayLength() > 0)
                {
                    var text = content_parts[0].GetProperty("text").GetString();
                    return text ?? "No response from Gemini";
                }
            }
            
            // 답변을 찾을 수 없는 경우
            return "No response from Gemini";
        }
        catch (Exception ex)
        {
            // 모든 예외를 포장하여 상위로 전달
            throw new Exception($"Error calling Gemini API: {ex.Message}");
        }
    }
    
    #endregion
}