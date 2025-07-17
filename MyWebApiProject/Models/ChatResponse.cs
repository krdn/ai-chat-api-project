namespace MyWebApiProject.Models;

/// <summary>
/// AI 챗봇의 응답을 나타내는 응답 모델 클래스
/// API 호출 결과를 JSON 형태로 클라이언트에게 반환합니다.
/// </summary>
/// <example>
/// 성공적인 응답 예시:
/// {
///   "answer": "안녕하세요! 저는 AI 어시스턴트입니다. 현재 날씨 정보는 제공할 수 없지만...",
///   "success": true,
///   "error": null
/// }
/// 
/// 실패한 응답 예시:
/// {
///   "answer": "",
///   "success": false,
///   "error": "Question is required"
/// }
/// </example>
public class ChatResponse
{
    /// <summary>
    /// AI가 생성한 답변 텍스트
    /// </summary>
    /// <remarks>
    /// - 성공적인 경우: AI가 생성한 실제 답변 내용
    /// - 실패한 경우: 빈 문자열 또는 기본 메시지
    /// - 모든 답변은 한국어로 제공됩니다
    /// </remarks>
    /// <value>AI의 답변 텍스트</value>
    public string Answer { get; set; } = string.Empty;
    
    /// <summary>
    /// API 호출 성공 여부를 나타내는 플래그
    /// </summary>
    /// <remarks>
    /// - true: API 호출이 성공하여 정상적인 답변을 받았음
    /// - false: API 호출 실패 또는 오류 발생
    /// </remarks>
    /// <value>성공 여부 (true/false)</value>
    public bool Success { get; set; }
    
    /// <summary>
    /// 오류 발생 시 에러 메시지
    /// </summary>
    /// <remarks>
    /// - Success가 true인 경우: null
    /// - Success가 false인 경우: 구체적인 오류 메시지
    /// - 일반적인 오류 유형:
    ///   - "Question is required": 질문이 비어있음
    ///   - "OpenAI API request failed with status: 429": API 사용량 제한 초과
    ///   - "Gemini API request failed with status: 400": 잘못된 API 키 또는 요청
    /// </remarks>
    /// <value>오류 메시지 또는 null</value>
    public string? Error { get; set; }
}