// ================================================================
// AI Chat API 프로젝트 - 메인 진입점
// ChatGPT와 Google Gemini API를 통합한 질문-답변 웹 API 서비스
// ================================================================

using MyWebApiProject.Models;
using MyWebApiProject.Services;

// ASP.NET Core 웹 애플리케이션 빌더 생성
var builder = WebApplication.CreateBuilder(args);

// ================================================================
// 의존성 주입 컨테이너에 서비스 등록
// ================================================================

// HTTP 클라이언트 팩토리와 함께 AI 서비스들을 등록
// AddHttpClient<T>()는 HttpClient의 수명주기를 자동으로 관리하고
// 연결 풀링, 재시도 정책 등을 제공합니다
builder.Services.AddHttpClient<ChatGptService>();    // ChatGPT API 서비스
builder.Services.AddHttpClient<GeminiService>();     // Google Gemini API 서비스

// API 탐색기 서비스 추가 (Swagger 문서 생성을 위해 필요)
builder.Services.AddEndpointsApiExplorer();

// Swagger 문서 생성 서비스 추가
builder.Services.AddSwaggerGen();

// 웹 애플리케이션 빌드
var app = builder.Build();

// ================================================================
// HTTP 요청 처리 파이프라인 구성
// ================================================================

// 개발 환경에서만 Swagger UI 활성화
if (app.Environment.IsDevelopment())
{
    // Swagger JSON 문서 생성 미들웨어
    app.UseSwagger();
    
    // Swagger UI 웹 인터페이스 활성화
    // 브라우저에서 /swagger로 접속하여 API 문서 확인 가능
    app.UseSwaggerUI();
}

// HTTP를 HTTPS로 리다이렉션하는 미들웨어
app.UseHttpsRedirection();

// ================================================================
// 미사용 코드 (WeatherForecast 예제)
// TODO: 향후 제거 예정
// ================================================================
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// ================================================================
// API 엔드포인트 정의
// ================================================================

// ChatGPT API 엔드포인트 - POST /chat
app.MapPost("/chat", async (ChatRequest request, ChatGptService chatGptService) =>
{
    try
    {
        // 입력 유효성 검사: 질문이 비어있거나 공백인지 확인
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Results.BadRequest(new ChatResponse
            {
                Success = false,
                Error = "Question is required"
            });
        }

        // ChatGPT API를 호출하여 답변 생성
        var answer = await chatGptService.GetAnswerAsync(request.Question);
        
        // 성공적인 응답 반환
        return Results.Ok(new ChatResponse
        {
            Answer = answer,
            Success = true
        });
    }
    catch (Exception ex)
    {
        // 예외 발생 시 500 에러와 함께 에러 메시지 반환
        return Results.Json(new ChatResponse
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("Chat")                    // 엔드포인트 이름 설정 (Swagger에서 사용)
.WithOpenApi();                      // OpenAPI 문서에 포함

// Google Gemini API 엔드포인트 - POST /gemini
app.MapPost("/gemini", async (ChatRequest request, GeminiService geminiService) =>
{
    try
    {
        // 입력 유효성 검사: 질문이 비어있거나 공백인지 확인
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Results.BadRequest(new ChatResponse
            {
                Success = false,
                Error = "Question is required"
            });
        }

        // Google Gemini API를 호출하여 답변 생성
        var answer = await geminiService.GetAnswerAsync(request.Question);
        
        // 성공적인 응답 반환
        return Results.Ok(new ChatResponse
        {
            Answer = answer,
            Success = true
        });
    }
    catch (Exception ex)
    {
        // 예외 발생 시 500 에러와 함께 에러 메시지 반환
        return Results.Json(new ChatResponse
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("Gemini")                  // 엔드포인트 이름 설정 (Swagger에서 사용)
.WithOpenApi();                      // OpenAPI 문서에 포함

// ================================================================
// 애플리케이션 실행
// ================================================================

// 웹 서버 시작 및 들어오는 HTTP 요청 처리 시작
app.Run();

// ================================================================
// 미사용 모델 클래스 (WeatherForecast 예제)
// TODO: 향후 제거 예정
// ================================================================

/// <summary>
/// 날씨 예보 정보를 나타내는 레코드 클래스
/// 이 클래스는 프로젝트 템플릿에서 제공되는 예제이며 실제로 사용되지 않습니다
/// </summary>
/// <param name="Date">날짜</param>
/// <param name="TemperatureC">섭씨 온도</param>
/// <param name="Summary">날씨 요약</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// 섭씨 온도를 화씨 온도로 변환하는 계산 속성
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
