using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MyWebApiProject.Services;

/// <summary>
/// OpenAI ChatGPT API와 상호작용하는 서비스 클래스
/// GPT-3.5-turbo 모델을 사용하여 자연어 질문에 대한 답변을 생성합니다.
/// </summary>
public class ChatGptService
{
    #region Private Fields
    
    /// <summary>
    /// HTTP 요청을 처리하는 클라이언트
    /// </summary>
    private readonly HttpClient _httpClient;
    
    /// <summary>
    /// OpenAI에서 발급받은 API 키
    /// </summary>
    private readonly string _apiKey;
    
    #endregion
    
    #region Constructor
    
    /// <summary>
    /// ChatGptService 생성자
    /// </summary>
    /// <param name="configuration">애플리케이션 설정 객체 (appsettings.json에서 API 키 읽기)</param>
    /// <param name="httpClient">DI 컨테이너에서 주입받는 HttpClient 인스턴스</param>
    /// <exception cref="ArgumentException">OpenAI API 키가 설정되지 않았을 때 발생</exception>
    public ChatGptService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        // appsettings.json에서 "OpenAI:ApiKey" 값을 읽어옵니다
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentException("OpenAI API key is not configured");
        
        // HTTP 클라이언트의 기본 요청 헤더에 인증 정보 추가
        // Bearer 토큰 방식으로 API 키를 전달합니다
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// 사용자 질문에 대한 ChatGPT AI의 답변을 비동기적으로 가져옵니다.
    /// </summary>
    /// <param name="question">사용자의 질문 텍스트</param>
    /// <returns>ChatGPT AI의 답변 텍스트</returns>
    /// <exception cref="Exception">API 호출 실패 시 발생</exception>
    public async Task<string> GetAnswerAsync(string question)
    {
        // OpenAI Chat Completions API 요청 본문 구성
        var requestBody = new
        {
            model = "gpt-3.5-turbo",  // 사용할 GPT 모델 (gpt-3.5-turbo는 비용 효율적)
            messages = new[]
            {
                // 시스템 메시지: AI의 역할과 행동 방식을 정의
                new { 
                    role = "system", 
                    content = "당신은 친절한 AI 어시스턴트입니다. 모든 답변을 한국어로 해주세요." 
                },
                // 사용자 메시지: 실제 질문 내용
                new { 
                    role = "user", 
                    content = question 
                }
            }
        };
        
        // 요청 본문을 JSON 문자열로 직렬화
        var json = JsonSerializer.Serialize(requestBody);
        
        // HTTP 요청 콘텐츠 생성 (UTF-8 인코딩, application/json 타입)
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        // OpenAI Chat Completions API에 POST 요청 전송
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        
        // HTTP 응답 상태 코드 확인
        if (!response.IsSuccessStatusCode)
        {
            // 실패 시 상태 코드와 함께 예외 발생
            throw new Exception($"OpenAI API request failed with status: {response.StatusCode}");
        }
        
        // 성공적인 응답 내용 읽기
        var responseContent = await response.Content.ReadAsStringAsync();
        
        // JSON 응답 파싱
        var jsonDocument = JsonDocument.Parse(responseContent);
        
        // OpenAI API 응답 구조에서 답변 텍스트 추출
        // 응답 구조: { choices: [{ message: { content: "답변 내용" } }] }
        var messageContent = jsonDocument.RootElement
            .GetProperty("choices")[0]        // 첫 번째 선택지 (보통 하나만 있음)
            .GetProperty("message")           // 메시지 객체
            .GetProperty("content")           // 실제 답변 내용
            .GetString();                     // 문자열로 변환
            
        // null인 경우 기본 메시지 반환
        return messageContent ?? "No response from OpenAI";
    }
    
    #endregion
}