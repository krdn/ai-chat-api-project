# AI Chat API Project

ChatGPT와 Google Gemini API를 사용한 질문-답변 시스템

## 📋 프로젝트 개요

이 프로젝트는 ChatGPT와 Google Gemini API를 통합한 웹 API 서비스입니다. 사용자는 두 개의 서로 다른 AI 모델에 질문하고 답변을 받을 수 있습니다.

## 🚀 주요 기능

- **ChatGPT API 연동**: OpenAI GPT-3.5-turbo 모델 사용
- **Google Gemini API 연동**: Google Gemini 1.5 Flash 모델 사용
- **RESTful API**: POST 방식의 간단한 API 엔드포인트
- **한국어 지원**: 모든 응답이 한국어로 출력
- **에러 처리**: 포괄적인 예외 처리 및 에러 응답
- **테스트 클라이언트**: 대화형 콘솔 테스트 애플리케이션

## 📁 프로젝트 구조

```
MyWebApiProject/
├── MyWebApiProject/          # 메인 웹 API 프로젝트
│   ├── Services/
│   │   ├── ChatGptService.cs # ChatGPT API 서비스
│   │   └── GeminiService.cs  # Gemini API 서비스
│   ├── Models/
│   │   ├── ChatRequest.cs    # 요청 모델
│   │   └── ChatResponse.cs   # 응답 모델
│   ├── Program.cs            # 메인 프로그램
│   └── appsettings.json      # 설정 파일
├── TestClient/               # 테스트 클라이언트 프로젝트
│   ├── Services/
│   │   └── ApiClient.cs      # API 호출 클라이언트
│   ├── Models/               # 모델 클래스
│   └── Program.cs            # 테스트 프로그램
└── README.md
```

## 🔧 설치 및 설정

### 1. 사전 요구사항

- .NET 8.0 SDK
- OpenAI API 키 (선택사항)
- Google AI Studio API 키

### 2. API 키 설정

1. **OpenAI API 키 발급** (선택사항):
   - https://platform.openai.com/api-keys 에서 발급

2. **Google Gemini API 키 발급**:
   - https://makersuite.google.com/app/apikey 에서 발급

3. **설정 파일 생성**:
   ```bash
   cp appsettings.example.json MyWebApiProject/appsettings.json
   ```

4. **API 키 입력**:
   ```json
   {
     "OpenAI": {
       "ApiKey": "YOUR_OPENAI_API_KEY_HERE"
     },
     "Gemini": {
       "ApiKey": "YOUR_GEMINI_API_KEY_HERE"
     }
   }
   ```

### 3. 프로젝트 빌드 및 실행

```bash
# 의존성 복원
dotnet restore

# 웹 API 서버 실행
cd MyWebApiProject
dotnet run --urls "http://localhost:5000"

# 테스트 클라이언트 실행 (새 터미널에서)
cd TestClient
dotnet run
```

## 🔌 API 엔드포인트

### ChatGPT API
```http
POST /chat
Content-Type: application/json

{
  "question": "안녕하세요, 어떻게 지내세요?"
}
```

### Gemini API
```http
POST /gemini
Content-Type: application/json

{
  "question": "파이썬으로 Hello World 출력하기"
}
```

### 응답 형식
```json
{
  "answer": "안녕하세요! 저는 AI 어시스턴트입니다. 잘 지내고 있습니다!",
  "success": true,
  "error": null
}
```

## 🧪 테스트 방법

### 1. 웹 API 테스트

**Swagger UI 사용**:
- http://localhost:5000/swagger 접속

**cURL 사용**:
```bash
# Gemini API 테스트
curl -X POST "http://localhost:5000/gemini" \
  -H "Content-Type: application/json" \
  -d '{"question": "안녕하세요"}'

# ChatGPT API 테스트
curl -X POST "http://localhost:5000/chat" \
  -H "Content-Type: application/json" \
  -d '{"question": "안녕하세요"}'
```

### 2. 테스트 클라이언트 사용

테스트 클라이언트는 5가지 모드를 제공합니다:

1. **ChatGPT API 단일 테스트**
2. **Gemini API 단일 테스트**
3. **ChatGPT 대화형 테스트**
4. **Gemini 대화형 테스트**
5. **프로그램 종료**

```bash
cd TestClient
dotnet run
```

## 🛠️ 기술 스택

- **Backend**: ASP.NET Core 8.0
- **HTTP Client**: System.Net.Http
- **JSON**: System.Text.Json
- **API Documentation**: Swagger/OpenAPI
- **Testing**: Console Application

## 📝 주요 특징

### 보안
- API 키는 환경 변수로 관리
- 민감한 정보는 .gitignore에 포함
- 예시 설정 파일 제공

### 성능
- HttpClient 재사용
- 비동기 처리
- 적절한 타임아웃 설정

### 사용성
- 한국어 응답 지원
- 포괄적인 에러 처리
- 사용자 친화적인 테스트 인터페이스

## 🐛 문제 해결

### OpenAI API 사용량 제한
- 429 에러 발생 시 Gemini API 사용 권장
- 새로운 API 키 발급 또는 대기 시간 필요

### 연결 오류
- API 서버가 실행 중인지 확인
- 방화벽 설정 확인
- 포트 충돌 시 다른 포트 사용

### API 키 오류
- 유효한 API 키인지 확인
- 설정 파일 경로 확인
- 키 형식 확인

## 🤝 기여하기

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 라이센스

이 프로젝트는 MIT 라이센스로 배포됩니다. 자세한 내용은 `LICENSE` 파일을 참조하세요.

## 📞 연락처

프로젝트에 대한 문의나 제안사항이 있으시면 이슈를 생성해주세요.

## 🙏 감사의 말

- OpenAI - ChatGPT API 제공
- Google - Gemini API 제공
- Microsoft - .NET 플랫폼 제공