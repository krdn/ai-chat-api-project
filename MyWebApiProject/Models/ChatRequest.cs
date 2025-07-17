namespace MyWebApiProject.Models;

/// <summary>
/// AI 챗봇에게 질문을 전송하기 위한 요청 모델 클래스
/// HTTP POST 요청의 본문(body)에 JSON 형태로 전송됩니다.
/// </summary>
/// <example>
/// JSON 예시:
/// {
///   "question": "안녕하세요, 오늘 날씨는 어떤가요?"
/// }
/// </example>
public class ChatRequest
{
    /// <summary>
    /// 사용자가 AI에게 묻고 싶은 질문 내용
    /// </summary>
    /// <remarks>
    /// - 빈 문자열이나 공백만 있는 경우 API에서 BadRequest(400) 응답을 반환합니다
    /// - 최대 길이 제한은 각 AI 모델의 토큰 제한을 따릅니다
    /// - 한국어, 영어 등 다양한 언어로 질문 가능합니다
    /// </remarks>
    /// <value>사용자 질문 텍스트</value>
    public string Question { get; set; } = string.Empty;
}