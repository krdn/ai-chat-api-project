namespace TestClient.Models;

/// <summary>
/// AI 챗봇에게 질문을 전송하기 위한 요청 모델 클래스 (테스트 클라이언트용)
/// API 서버의 ChatRequest 모델과 동일한 구조를 가집니다.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// 사용자가 AI에게 묻고 싶은 질문 내용
    /// </summary>
    /// <value>사용자 질문 텍스트</value>
    public string Question { get; set; } = string.Empty;
}