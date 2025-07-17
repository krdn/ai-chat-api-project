# 🚀 AI Chat API 프로젝트 개발 지침

## 📋 개요

이 문서는 AI Chat API 프로젝트의 개발, 확장, 유지보수에 대한 종합적인 지침을 제공합니다. 본 프로젝트는 외부 API 통합을 위한 재사용 가능한 템플릿으로 설계되었습니다.

## 🎯 프로젝트 목표

- **외부 API 통합**: 다양한 AI 서비스 (ChatGPT, Gemini) 통합
- **확장 가능한 아키텍처**: 새로운 API 서비스 쉽게 추가 가능
- **재사용 가능한 템플릿**: 다른 프로젝트의 기반으로 활용
- **테스트 우선 개발**: 완전한 테스트 인프라 제공

## 🏗️ 프로젝트 구조

```
MyWebApiProject/
├── MyWebApiProject/          # 메인 웹 API 프로젝트
│   ├── Services/            # 외부 API 서비스 구현
│   │   ├── ChatGptService.cs
│   │   └── GeminiService.cs
│   ├── Models/              # 요청/응답 모델
│   │   ├── ChatRequest.cs
│   │   └── ChatResponse.cs
│   ├── Program.cs           # 애플리케이션 진입점
│   └── appsettings.json     # 설정 파일
├── TestClient/              # 테스트 클라이언트
│   ├── Services/
│   │   └── ApiClient.cs
│   ├── Models/
│   └── Program.cs
├── README.md                # 프로젝트 문서
├── PROJECT_GUIDELINES.md    # 개발 지침 (이 문서)
└── .gitignore              # Git 무시 파일
```

## 🔧 핵심 패턴 및 아키텍처

### 1. 외부 API 서비스 패턴

**기본 구조:**
```csharp
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public ExternalApiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["ServiceName:ApiKey"];
    }
    
    public async Task<ResponseModel> CallApiAsync(RequestModel request)
    {
        // API 호출 로직
    }
}
```

**핵심 원칙:**
- HttpClient는 DI 컨테이너에서 주입
- API 키는 설정 파일에서 관리
- 비동기 패턴 사용
- 상세한 에러 처리

### 2. 요청/응답 모델 패턴

**요청 모델:**
```csharp
public class ApiRequest
{
    public string Question { get; set; } = string.Empty;
    
    // 유효성 검사 메서드
    public bool IsValid() => !string.IsNullOrWhiteSpace(Question);
}
```

**응답 모델:**
```csharp
public class ApiResponse
{
    public string Answer { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}
```

### 3. API 엔드포인트 패턴

**표준 엔드포인트 구조:**
```csharp
app.MapPost("/api-endpoint", async (RequestModel request, ServiceClass service) =>
{
    try
    {
        // 1. 입력 유효성 검사
        if (!request.IsValid())
        {
            return Results.BadRequest(new ResponseModel
            {
                Success = false,
                Error = "Invalid request"
            });
        }

        // 2. 서비스 호출
        var response = await service.ProcessAsync(request);
        
        // 3. 성공 응답
        return Results.Ok(response);
    }
    catch (Exception ex)
    {
        // 4. 오류 처리
        return Results.Json(new ResponseModel
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("EndpointName")
.WithOpenApi();
```

## 🚀 새로운 API 서비스 추가 가이드

### 1. 서비스 클래스 생성

**파일 위치:** `MyWebApiProject/Services/NewApiService.cs`

```csharp
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MyWebApiProject.Services;

/// <summary>
/// 새로운 API 서비스 클래스
/// </summary>
public class NewApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.newservice.com/v1";
    
    public NewApiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["NewApi:ApiKey"] ?? 
                  throw new ArgumentException("NewApi API key is not configured");
    }
    
    public async Task<string> GetAnswerAsync(string question)
    {
        try
        {
            // API 호출 로직 구현
            // 기존 서비스들의 패턴을 참고
        }
        catch (Exception ex)
        {
            throw new Exception($"Error calling NewApi: {ex.Message}");
        }
    }
}
```

### 2. 설정 파일 업데이트

**appsettings.json 수정:**
```json
{
  "NewApi": {
    "ApiKey": "YOUR_NEW_API_KEY_HERE"
  }
}
```

### 3. 서비스 등록

**Program.cs 수정:**
```csharp
// 서비스 등록
builder.Services.AddHttpClient<NewApiService>();

// 엔드포인트 추가
app.MapPost("/newapi", async (ChatRequest request, NewApiService service) =>
{
    // 기존 엔드포인트 패턴 따라 구현
})
.WithName("NewApi")
.WithOpenApi();
```

### 4. 테스트 클라이언트 업데이트

**TestClient/Services/ApiClient.cs 수정:**
```csharp
public async Task<ChatResponse> CallNewApiAsync(string question)
{
    return await CallApiAsync("/newapi", question);
}
```

**TestClient/Program.cs 수정:**
```csharp
// 메뉴에 새로운 옵션 추가
Console.WriteLine("6. NewApi 테스트");

// switch 문에 케이스 추가
case "6":
    await TestNewApi();
    break;
```

## 🔍 코드 품질 지침

### 1. 코딩 스타일

**명명 규칙:**
- 클래스: PascalCase (`ChatGptService`)
- 메서드: PascalCase (`GetAnswerAsync`)
- 변수: camelCase (`apiKey`)
- 상수: PascalCase (`BaseUrl`)

**주석 규칙:**
- 모든 public 클래스와 메서드에 XML 문서 주석 추가
- 복잡한 로직에는 인라인 주석 추가
- 파일 상단에 목적 설명 주석 추가

### 2. 오류 처리

**표준 오류 처리 패턴:**
```csharp
try
{
    // 메인 로직
}
catch (HttpRequestException ex)
{
    // HTTP 관련 오류
    throw new Exception($"HTTP request failed: {ex.Message}");
}
catch (JsonException ex)
{
    // JSON 파싱 오류
    throw new Exception($"JSON parsing failed: {ex.Message}");
}
catch (Exception ex)
{
    // 일반적인 오류
    throw new Exception($"API call failed: {ex.Message}");
}
```

### 3. 비동기 프로그래밍

**비동기 메서드 규칙:**
- 모든 I/O 작업은 비동기로 처리
- 메서드 이름에 `Async` 접미사 추가
- `ConfigureAwait(false)` 사용 고려
- CancellationToken 매개변수 추가 권장

## 🧪 테스트 지침

### 1. 테스트 구조

**테스트 클라이언트 기능:**
- 단일 API 테스트
- 대화형 테스트
- 오류 시나리오 테스트
- 성능 테스트

**테스트 시나리오:**
1. 정상적인 요청/응답 테스트
2. 빈 질문 입력 테스트
3. 잘못된 API 키 테스트
4. 네트워크 오류 테스트
5. 타임아웃 테스트

### 2. 테스트 실행 방법

```bash
# 웹 API 서버 실행
cd MyWebApiProject
dotnet run --urls "http://localhost:5000"

# 테스트 클라이언트 실행
cd TestClient
dotnet run
```

## 🔧 환경 설정 가이드

### 1. 개발 환경 설정

**필수 요구사항:**
- .NET 8.0 SDK
- Visual Studio Code 또는 Visual Studio
- Git

**API 키 설정:**
1. OpenAI API 키 발급: https://platform.openai.com/api-keys
2. Google AI Studio API 키 발급: https://makersuite.google.com/app/apikey
3. `appsettings.json`에 API 키 설정

### 2. 배포 환경 설정

**환경 변수 설정:**
```bash
export OPENAI_API_KEY="your-openai-api-key"
export GEMINI_API_KEY="your-gemini-api-key"
```

**Docker 배포:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "MyWebApiProject.dll"]
```

## 📊 성능 최적화 가이드

### 1. HTTP 클라이언트 최적화

**HttpClient 설정:**
```csharp
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

### 2. 응답 캐싱

**메모리 캐싱 추가:**
```csharp
builder.Services.AddMemoryCache();

// 서비스에서 캐싱 사용
public async Task<string> GetCachedAnswerAsync(string question)
{
    var cacheKey = $"answer:{question.GetHashCode()}";
    
    if (_cache.TryGetValue(cacheKey, out string cachedAnswer))
    {
        return cachedAnswer;
    }
    
    var answer = await GetAnswerAsync(question);
    _cache.Set(cacheKey, answer, TimeSpan.FromMinutes(10));
    
    return answer;
}
```

## 🔐 보안 가이드

### 1. API 키 관리

**보안 원칙:**
- API 키는 절대 코드에 하드코딩하지 않음
- 환경 변수 또는 설정 파일 사용
- 프로덕션에서는 Azure Key Vault 등 사용
- Git에 API 키 커밋 방지 (.gitignore 설정)

### 2. HTTPS 및 CORS 설정

**HTTPS 강제:**
```csharp
app.UseHttpsRedirection();
```

**CORS 설정:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://yourdomain.com")
                         .AllowAnyHeader()
                         .AllowAnyMethod());
});
```

## 📈 모니터링 및 로깅

### 1. 로깅 설정

**구조화된 로깅:**
```csharp
// 서비스에서 로깅 사용
public class ChatGptService
{
    private readonly ILogger<ChatGptService> _logger;
    
    public ChatGptService(ILogger<ChatGptService> logger, ...)
    {
        _logger = logger;
    }
    
    public async Task<string> GetAnswerAsync(string question)
    {
        _logger.LogInformation("Processing question: {Question}", question);
        
        try
        {
            // API 호출
            _logger.LogInformation("API call successful");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API call failed");
            throw;
        }
    }
}
```

### 2. 메트릭 수집

**사용량 추적:**
```csharp
// 카운터 추가
private static readonly Counter<int> RequestCounter = 
    Meter.CreateCounter<int>("api_requests_total");

// 요청 시 카운터 증가
RequestCounter.Add(1, new KeyValuePair<string, object>("endpoint", "/chat"));
```

## 🔄 버전 관리 가이드

### 1. Git 브랜치 전략

**브랜치 구조:**
- `main`: 안정적인 프로덕션 코드
- `develop`: 개발 중인 기능들
- `feature/*`: 새로운 기능 개발
- `hotfix/*`: 긴급 수정

### 2. 커밋 메시지 규칙

**커밋 메시지 형식:**
```
feat: add new API service for Claude
fix: resolve API timeout issue
docs: update README with new installation guide
refactor: improve error handling in services
test: add unit tests for ChatGPT service
```

## 📚 확장 가능한 아키텍처 패턴

### 1. 플러그인 아키텍처

**인터페이스 정의:**
```csharp
public interface IApiPlugin
{
    string Name { get; }
    string Version { get; }
    Task<IApiResponse> ProcessAsync(IApiRequest request);
}
```

### 2. 이벤트 기반 아키텍처

**이벤트 시스템:**
```csharp
public class ApiEventBus
{
    public event EventHandler<ApiRequestEvent> RequestReceived;
    public event EventHandler<ApiResponseEvent> ResponseGenerated;
    
    // 이벤트 발생 및 처리 로직
}
```

### 3. 마이크로서비스 준비

**서비스 분리 가이드:**
1. 각 API 서비스를 별도 프로젝트로 분리
2. 공통 라이브러리 NuGet 패키지화
3. API Gateway 패턴 적용
4. 서비스 간 통신 (HTTP/gRPC)

## 🎯 템플릿 활용 가이드

### 1. 새로운 프로젝트 생성

**단계별 가이드:**
1. 현재 프로젝트 복사
2. 프로젝트 이름 및 네임스페이스 변경
3. 필요 없는 서비스 제거
4. 새로운 API 서비스 추가
5. 설정 파일 수정
6. 테스트 및 문서 업데이트

### 2. 활용 사례

**적용 가능한 분야:**
- 소셜 미디어 통합 API
- 결제 시스템 통합
- 클라우드 서비스 관리
- 데이터 분석 플랫폼
- IoT 디바이스 관리
- 알림 시스템 통합

## 🚨 문제 해결 가이드

### 1. 일반적인 문제

**API 호출 실패:**
- API 키 확인
- 네트워크 연결 확인
- 요청 형식 검증
- 로그 확인

**성능 문제:**
- HttpClient 재사용 확인
- 캐싱 적용 검토
- 타임아웃 설정 조정
- 비동기 패턴 확인

### 2. 디버깅 팁

**로깅 활용:**
```csharp
// 디버깅을 위한 상세 로깅
_logger.LogDebug("Request body: {RequestBody}", jsonRequest);
_logger.LogDebug("Response: {Response}", responseContent);
```

**테스트 도구:**
- Postman으로 API 엔드포인트 테스트
- Swagger UI로 API 문서 확인
- 테스트 클라이언트로 시나리오 테스트

## 📝 체크리스트

### 새로운 API 서비스 추가 시

- [ ] 서비스 클래스 생성
- [ ] 설정 파일 업데이트
- [ ] Program.cs에 서비스 등록
- [ ] API 엔드포인트 추가
- [ ] 테스트 클라이언트 업데이트
- [ ] 문서 업데이트
- [ ] 단위 테스트 작성
- [ ] 통합 테스트 실행

### 프로덕션 배포 시

- [ ] API 키 보안 설정
- [ ] HTTPS 설정
- [ ] 로깅 설정
- [ ] 모니터링 설정
- [ ] 성능 테스트
- [ ] 보안 검토
- [ ] 문서 최종 검토

## 🔮 향후 계획

### 단기 목표 (1-3개월)
- 추가 AI 서비스 통합 (Claude, Bard 등)
- 성능 최적화 및 캐싱 구현
- 종합적인 테스트 스위트 구축

### 중기 목표 (3-6개월)
- 마이크로서비스 아키텍처 적용
- 클라우드 배포 자동화
- 모니터링 및 알림 시스템 구축

### 장기 목표 (6-12개월)
- 플러그인 아키텍처 구현
- 다국어 지원
- 엔터프라이즈 기능 추가

## 📞 지원 및 문의

**문제 보고:**
- GitHub Issues: https://github.com/krdn/ai-chat-api-project/issues
- 이메일: [프로젝트 관리자 이메일]

**기여 방법:**
1. 프로젝트 포크
2. 기능 브랜치 생성
3. 변경사항 커밋
4. 풀 리퀘스트 생성

---

**마지막 업데이트:** 2025년 1월 17일  
**버전:** 1.0.0  
**작성자:** Claude Code Assistant

이 지침서는 프로젝트의 발전에 따라 지속적으로 업데이트됩니다.