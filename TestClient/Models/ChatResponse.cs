namespace TestClient.Models;

/// <summary>
/// AI 챗봇의 응답을 나타내는 응답 모델 클래스 (테스트 클라이언트용)
/// API 서버의 ChatResponse 모델과 동일한 구조를 가집니다.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// AI가 생성한 답변 텍스트
    /// </summary>
    /// <value>AI의 답변 텍스트</value>
    public string Answer { get; set; } = string.Empty;
    
    /// <summary>
    /// API 호출 성공 여부를 나타내는 플래그
    /// </summary>
    /// <value>성공 여부 (true/false)</value>
    public bool Success { get; set; }
    
    /// <summary>
    /// 오류 발생 시 에러 메시지
    /// </summary>
    /// <value>오류 메시지 또는 null</value>
    public string? Error { get; set; }
}