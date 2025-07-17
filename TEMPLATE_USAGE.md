# 🎯 프로젝트 템플릿 활용 가이드

## 📋 개요

이 문서는 AI Chat API 프로젝트를 다른 용도로 활용하기 위한 템플릿 가이드입니다. 현재 프로젝트 구조를 기반으로 다양한 외부 API 통합 프로젝트를 빠르게 개발할 수 있습니다.

## 🚀 빠른 시작 가이드

### 1. 프로젝트 복사 및 초기 설정

```bash
# 1. 프로젝트 복사
git clone https://github.com/krdn/ai-chat-api-project.git MyNewProject
cd MyNewProject

# 2. Git 히스토리 초기화
rm -rf .git
git init
git add .
git commit -m "Initial commit from template"

# 3. 의존성 복원
dotnet restore

# 4. 빌드 테스트
dotnet build
```

### 2. 프로젝트 커스터마이징

```bash
# 1. 네임스페이스 변경
find . -name "*.cs" -exec sed -i 's/MyWebApiProject/YourProjectName/g' {} \;

# 2. 솔루션 파일 이름 변경
mv MyWebApiProject.sln YourProjectName.sln

# 3. 프로젝트 폴더 이름 변경
mv MyWebApiProject YourProjectName
mv TestClient YourProjectTestClient
```

## 🔧 다양한 활용 시나리오

### 1. 소셜 미디어 통합 API

**목표:** Twitter, Facebook, Instagram API 통합

**구현 단계:**

#### A. TwitterService 추가

```csharp
// Services/TwitterService.cs
public class TwitterService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private const string BaseUrl = "https://api.twitter.com/2";
    
    public TwitterService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Twitter:ApiKey"];
        _apiSecret = configuration["Twitter:ApiSecret"];
        
        // OAuth 인증 설정
        SetupOAuthAuthentication();
    }
    
    public async Task<TwitterResponse> PostTweetAsync(string message)
    {
        var requestBody = new
        {
            text = message
        };
        
        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{BaseUrl}/tweets", content);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Twitter API failed: {response.StatusCode}");
        }
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var twitterResponse = JsonSerializer.Deserialize<TwitterResponse>(responseContent);
        
        return twitterResponse;
    }
}
```

#### B. 모델 클래스 추가

```csharp
// Models/TwitterRequest.cs
public class TwitterRequest
{
    public string Message { get; set; } = string.Empty;
    public List<string> MediaUrls { get; set; } = new();
    public bool IsReply { get; set; }
    public string? ReplyToId { get; set; }
}

// Models/TwitterResponse.cs
public class TwitterResponse
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
```

#### C. API 엔드포인트 추가

```csharp
// Program.cs에 추가
app.MapPost("/twitter/post", async (TwitterRequest request, TwitterService twitterService) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return Results.BadRequest(new TwitterResponse
            {
                Success = false,
                Error = "Message is required"
            });
        }

        var result = await twitterService.PostTweetAsync(request.Message);
        
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Json(new TwitterResponse
        {
            Success = false,
            Error = ex.Message
        }, statusCode: 500);
    }
})
.WithName("PostTweet")
.WithOpenApi();
```

### 2. 결제 시스템 통합 API

**목표:** PayPal, Stripe, 토스페이 API 통합

**구현 예시:**

#### A. PaymentService 구조

```csharp
// Services/PaymentService.cs
public abstract class PaymentService
{
    protected readonly HttpClient _httpClient;
    protected readonly string _apiKey;
    protected readonly string _baseUrl;
    
    protected PaymentService(HttpClient httpClient, string apiKey, string baseUrl)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
        _baseUrl = baseUrl;
    }
    
    public abstract Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    public abstract Task<PaymentResponse> RefundPaymentAsync(string transactionId);
    public abstract Task<PaymentResponse> GetPaymentStatusAsync(string transactionId);
}

// Services/StripePaymentService.cs
public class StripePaymentService : PaymentService
{
    public StripePaymentService(IConfiguration configuration, HttpClient httpClient)
        : base(httpClient, configuration["Stripe:ApiKey"], "https://api.stripe.com/v1")
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    public override async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        // Stripe 결제 처리 로직
        var stripeRequest = new
        {
            amount = request.Amount * 100, // cents
            currency = request.Currency.ToLower(),
            payment_method = request.PaymentMethodId,
            confirm = true
        };
        
        var json = JsonSerializer.Serialize(stripeRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}/payment_intents", content);
        
        // 응답 처리...
        return new PaymentResponse { /* 결과 매핑 */ };
    }
}
```

#### B. 결제 모델 정의

```csharp
// Models/PaymentRequest.cs
public class PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethodId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

// Models/PaymentResponse.cs
public class PaymentResponse
{
    public string TransactionId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Refunded,
    Cancelled
}
```

### 3. 클라우드 서비스 통합 API

**목표:** AWS, Azure, GCP API 통합

**구현 예시:**

#### A. CloudStorageService 구조

```csharp
// Services/CloudStorageService.cs
public interface ICloudStorageService
{
    Task<CloudUploadResponse> UploadFileAsync(CloudUploadRequest request);
    Task<CloudDownloadResponse> DownloadFileAsync(string fileId);
    Task<CloudDeleteResponse> DeleteFileAsync(string fileId);
    Task<CloudListResponse> ListFilesAsync(string folder = "");
}

// Services/AWSStorageService.cs
public class AWSStorageService : ICloudStorageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    
    public AWSStorageService(IConfiguration configuration)
    {
        var awsConfig = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };
        
        _s3Client = new AmazonS3Client(
            configuration["AWS:AccessKey"],
            configuration["AWS:SecretKey"],
            awsConfig
        );
        
        _bucketName = configuration["AWS:BucketName"];
    }
    
    public async Task<CloudUploadResponse> UploadFileAsync(CloudUploadRequest request)
    {
        try
        {
            var uploadRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = request.FileName,
                InputStream = request.FileStream,
                ContentType = request.ContentType
            };
            
            var response = await _s3Client.PutObjectAsync(uploadRequest);
            
            return new CloudUploadResponse
            {
                FileId = request.FileName,
                Url = $"https://{_bucketName}.s3.amazonaws.com/{request.FileName}",
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new CloudUploadResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
}
```

### 4. 데이터 분석 플랫폼 API

**목표:** Google Analytics, Adobe Analytics, Mixpanel API 통합

**구현 예시:**

#### A. AnalyticsService 구조

```csharp
// Services/AnalyticsService.cs
public abstract class AnalyticsService
{
    protected readonly HttpClient _httpClient;
    protected readonly string _apiKey;
    
    protected AnalyticsService(HttpClient httpClient, string apiKey)
    {
        _httpClient = httpClient;
        _apiKey = apiKey;
    }
    
    public abstract Task<AnalyticsResponse> GetPageViewsAsync(AnalyticsRequest request);
    public abstract Task<AnalyticsResponse> GetUserSessionsAsync(AnalyticsRequest request);
    public abstract Task<AnalyticsResponse> GetConversionRatesAsync(AnalyticsRequest request);
}

// Services/GoogleAnalyticsService.cs
public class GoogleAnalyticsService : AnalyticsService
{
    private const string BaseUrl = "https://analyticsreporting.googleapis.com/v4";
    
    public GoogleAnalyticsService(IConfiguration configuration, HttpClient httpClient)
        : base(httpClient, configuration["GoogleAnalytics:ApiKey"])
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    
    public override async Task<AnalyticsResponse> GetPageViewsAsync(AnalyticsRequest request)
    {
        var reportRequest = new
        {
            reportRequests = new[]
            {
                new
                {
                    viewId = request.ViewId,
                    dateRanges = new[]
                    {
                        new
                        {
                            startDate = request.StartDate.ToString("yyyy-MM-dd"),
                            endDate = request.EndDate.ToString("yyyy-MM-dd")
                        }
                    },
                    metrics = new[]
                    {
                        new { expression = "ga:pageviews" }
                    },
                    dimensions = new[]
                    {
                        new { name = "ga:pagePath" }
                    }
                }
            }
        };
        
        var json = JsonSerializer.Serialize(reportRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{BaseUrl}/reports:batchGet", content);
        
        // 응답 처리 로직...
        return new AnalyticsResponse { /* 결과 매핑 */ };
    }
}
```

## 🔄 템플릿 확장 패턴

### 1. 새로운 서비스 추가 템플릿

```csharp
// Services/NewApiService.cs 템플릿
public class NewApiService
{
    #region Private Fields
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.newservice.com";
    #endregion
    
    #region Constructor
    public NewApiService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["NewService:ApiKey"] ?? 
                  throw new ArgumentException("NewService API key is not configured");
        
        // 필요한 경우 헤더 설정
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }
    #endregion
    
    #region Public Methods
    public async Task<NewServiceResponse> ProcessAsync(NewServiceRequest request)
    {
        try
        {
            // 1. 요청 검증
            if (!request.IsValid())
            {
                throw new ArgumentException("Invalid request");
            }
            
            // 2. API 호출 준비
            var apiRequest = MapToApiRequest(request);
            var json = JsonSerializer.Serialize(apiRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // 3. API 호출
            var response = await _httpClient.PostAsync($"{BaseUrl}/endpoint", content);
            
            // 4. 응답 처리
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API call failed: {response.StatusCode}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponseModel>(responseContent);
            
            // 5. 결과 매핑
            return MapToServiceResponse(apiResponse);
        }
        catch (Exception ex)
        {
            return new NewServiceResponse
            {
                Success = false,
                Error = ex.Message
            };
        }
    }
    #endregion
    
    #region Private Methods
    private object MapToApiRequest(NewServiceRequest request)
    {
        // 요청 객체 매핑 로직
        return new { /* 매핑된 객체 */ };
    }
    
    private NewServiceResponse MapToServiceResponse(ApiResponseModel apiResponse)
    {
        // 응답 객체 매핑 로직
        return new NewServiceResponse { /* 매핑된 객체 */ };
    }
    #endregion
}
```

### 2. 설정 파일 템플릿

```json
// appsettings.json 확장 템플릿
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  
  // 기존 AI 서비스들
  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY",
    "Model": "gpt-3.5-turbo",
    "MaxTokens": 1000,
    "Temperature": 0.7
  },
  "Gemini": {
    "ApiKey": "YOUR_GEMINI_API_KEY",
    "Model": "gemini-1.5-flash",
    "MaxTokens": 1024,
    "Temperature": 0.7
  },
  
  // 새로운 서비스들
  "Twitter": {
    "ApiKey": "YOUR_TWITTER_API_KEY",
    "ApiSecret": "YOUR_TWITTER_API_SECRET",
    "AccessToken": "YOUR_TWITTER_ACCESS_TOKEN",
    "AccessTokenSecret": "YOUR_TWITTER_ACCESS_TOKEN_SECRET"
  },
  "Stripe": {
    "ApiKey": "YOUR_STRIPE_SECRET_KEY",
    "PublishableKey": "YOUR_STRIPE_PUBLISHABLE_KEY",
    "WebhookSecret": "YOUR_STRIPE_WEBHOOK_SECRET"
  },
  "AWS": {
    "AccessKey": "YOUR_AWS_ACCESS_KEY",
    "SecretKey": "YOUR_AWS_SECRET_KEY",
    "BucketName": "YOUR_S3_BUCKET_NAME",
    "Region": "us-east-1"
  },
  "GoogleAnalytics": {
    "ApiKey": "YOUR_GA_API_KEY",
    "ViewId": "YOUR_GA_VIEW_ID",
    "ServiceAccountFile": "path/to/service-account.json"
  }
}
```

### 3. 테스트 클라이언트 확장 템플릿

```csharp
// TestClient/Program.cs 확장 템플릿
Console.WriteLine("=== Multi-Service API 테스트 클라이언트 ===");
Console.WriteLine("1. ChatGPT API 테스트");
Console.WriteLine("2. Gemini API 테스트");
Console.WriteLine("3. Twitter API 테스트");
Console.WriteLine("4. Payment API 테스트");
Console.WriteLine("5. Cloud Storage API 테스트");
Console.WriteLine("6. Analytics API 테스트");
Console.WriteLine("7. 통합 워크플로우 테스트");
Console.WriteLine("8. 종료");

// 새로운 테스트 함수들
async Task TestTwitterApi()
{
    Console.WriteLine("\n=== Twitter API 테스트 ===");
    Console.Write("트윗 내용을 입력하세요: ");
    var message = Console.ReadLine();
    
    var request = new TwitterRequest { Message = message };
    var response = await apiClient.CallTwitterApiAsync(request);
    
    if (response.Success)
    {
        Console.WriteLine($"트윗 성공! ID: {response.Id}");
    }
    else
    {
        Console.WriteLine($"오류: {response.Error}");
    }
}

async Task TestPaymentApi()
{
    Console.WriteLine("\n=== Payment API 테스트 ===");
    Console.Write("결제 금액을 입력하세요: ");
    var amount = decimal.Parse(Console.ReadLine() ?? "0");
    
    var request = new PaymentRequest 
    { 
        Amount = amount,
        Currency = "USD",
        Description = "테스트 결제"
    };
    
    var response = await apiClient.CallPaymentApiAsync(request);
    
    if (response.Success)
    {
        Console.WriteLine($"결제 성공! 거래 ID: {response.TransactionId}");
    }
    else
    {
        Console.WriteLine($"오류: {response.Error}");
    }
}
```

## 🔧 커스터마이징 가이드

### 1. 브랜딩 및 명명 규칙

```bash
# 프로젝트 이름 일괄 변경 스크립트
#!/bin/bash
OLD_NAME="MyWebApiProject"
NEW_NAME="YourProjectName"

# 파일 내용 변경
find . -type f -name "*.cs" -exec sed -i "s/$OLD_NAME/$NEW_NAME/g" {} \;
find . -type f -name "*.csproj" -exec sed -i "s/$OLD_NAME/$NEW_NAME/g" {} \;
find . -type f -name "*.sln" -exec sed -i "s/$OLD_NAME/$NEW_NAME/g" {} \;

# 파일 및 폴더 이름 변경
mv $OLD_NAME $NEW_NAME
mv "${OLD_NAME}.sln" "${NEW_NAME}.sln"
```

### 2. 도메인 특화 확장

```csharp
// 도메인 별 기본 인터페이스 정의
public interface IApiService<TRequest, TResponse>
{
    Task<TResponse> ProcessAsync(TRequest request);
    Task<bool> ValidateAsync(TRequest request);
    Task<TResponse> HandleErrorAsync(Exception ex);
}

// 도메인 별 기본 구현
public abstract class BaseApiService<TRequest, TResponse> : IApiService<TRequest, TResponse>
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;
    
    protected BaseApiService(HttpClient httpClient, ILogger logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
    }
    
    public abstract Task<TResponse> ProcessAsync(TRequest request);
    
    public virtual async Task<bool> ValidateAsync(TRequest request)
    {
        // 기본 검증 로직
        return true;
    }
    
    public virtual async Task<TResponse> HandleErrorAsync(Exception ex)
    {
        _logger.LogError(ex, "API call failed");
        
        // 기본 오류 응답 생성
        return CreateErrorResponse(ex.Message);
    }
    
    protected abstract TResponse CreateErrorResponse(string error);
}
```

## 📊 성능 최적화 템플릿

### 1. 캐싱 레이어 추가

```csharp
// Services/CachedApiService.cs
public class CachedApiService<TRequest, TResponse> : IApiService<TRequest, TResponse>
{
    private readonly IApiService<TRequest, TResponse> _innerService;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration;
    
    public CachedApiService(IApiService<TRequest, TResponse> innerService, 
                           IMemoryCache cache, 
                           TimeSpan cacheExpiration)
    {
        _innerService = innerService;
        _cache = cache;
        _cacheExpiration = cacheExpiration;
    }
    
    public async Task<TResponse> ProcessAsync(TRequest request)
    {
        var cacheKey = GenerateCacheKey(request);
        
        if (_cache.TryGetValue(cacheKey, out TResponse cachedResponse))
        {
            return cachedResponse;
        }
        
        var response = await _innerService.ProcessAsync(request);
        
        _cache.Set(cacheKey, response, _cacheExpiration);
        
        return response;
    }
    
    private string GenerateCacheKey(TRequest request)
    {
        return $"{typeof(TRequest).Name}:{request.GetHashCode()}";
    }
}
```

### 2. 비동기 처리 패턴

```csharp
// Services/AsyncApiService.cs
public class AsyncApiService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    
    public async Task<string> ProcessAsync(ApiRequest request)
    {
        var taskId = Guid.NewGuid().ToString();
        
        // 백그라운드 큐에 작업 추가
        _taskQueue.QueueBackgroundWorkItem(async cancellationToken =>
        {
            try
            {
                var result = await ProcessInternalAsync(request);
                // 결과 저장 (DB, 캐시 등)
                await SaveResultAsync(taskId, result);
            }
            catch (Exception ex)
            {
                // 오류 저장
                await SaveErrorAsync(taskId, ex);
            }
        });
        
        return taskId; // 클라이언트에 작업 ID 반환
    }
    
    public async Task<ApiResponse> GetResultAsync(string taskId)
    {
        // 작업 결과 조회
        return await LoadResultAsync(taskId);
    }
}
```

## 🎯 배포 및 운영 템플릿

### 1. Docker 컨테이너화

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YourProjectName/YourProjectName.csproj", "YourProjectName/"]
RUN dotnet restore "YourProjectName/YourProjectName.csproj"
COPY . .
WORKDIR "/src/YourProjectName"
RUN dotnet build "YourProjectName.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YourProjectName.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourProjectName.dll"]
```

### 2. Kubernetes 배포 설정

```yaml
# k8s-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: your-api-deployment
spec:
  replicas: 3
  selector:
    matchLabels:
      app: your-api
  template:
    metadata:
      labels:
        app: your-api
    spec:
      containers:
      - name: your-api
        image: your-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: OPENAI_API_KEY
          valueFrom:
            secretKeyRef:
              name: api-secrets
              key: openai-api-key
---
apiVersion: v1
kind: Service
metadata:
  name: your-api-service
spec:
  selector:
    app: your-api
  ports:
  - port: 80
    targetPort: 80
  type: LoadBalancer
```

---

**마지막 업데이트:** 2025년 1월 17일  
**버전:** 1.0.0  
**작성자:** Claude Code Assistant

이 템플릿 가이드는 프로젝트의 발전에 따라 지속적으로 업데이트됩니다.