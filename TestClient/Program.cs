// ================================================================
// AI Chat API 테스트 클라이언트 - 메인 진입점
// ChatGPT와 Google Gemini API를 테스트하는 콘솔 애플리케이션
// ================================================================

using TestClient.Services;

// ================================================================
// 프로그램 시작 - 메인 메뉴 표시
// ================================================================

Console.WriteLine("=== AI Chat API 테스트 클라이언트 ===");
Console.WriteLine("1. ChatGPT API 테스트");
Console.WriteLine("2. Gemini API 테스트");
Console.WriteLine("3. 대화형 테스트 (ChatGPT)");
Console.WriteLine("4. 대화형 테스트 (Gemini)");
Console.WriteLine("5. 종료");
Console.WriteLine();

// API 클라이언트 인스턴스 생성
var apiClient = new ApiClient();

// ================================================================
// 메인 루프 - 사용자 선택 처리
// ================================================================

while (true)
{
    Console.Write("선택하세요 (1-5): ");
    var choice = Console.ReadLine();

    // 사용자 선택에 따른 기능 실행
    switch (choice)
    {
        case "1":
            await TestChatGpt();        // ChatGPT 단일 테스트
            break;
        case "2":
            await TestGemini();         // Gemini 단일 테스트
            break;
        case "3":
            await InteractiveChat("ChatGPT");   // ChatGPT 대화형 테스트
            break;
        case "4":
            await InteractiveChat("Gemini");    // Gemini 대화형 테스트
            break;
        case "5":
            Console.WriteLine("프로그램을 종료합니다.");
            apiClient.Dispose();        // 리소스 해제
            return;                     // 프로그램 종료
        default:
            Console.WriteLine("잘못된 선택입니다. 다시 선택해주세요.");
            break;
    }
    
    Console.WriteLine();    // 메뉴 간 구분을 위한 빈 줄
}

// ================================================================
// 로컬 함수 정의 - 각 테스트 기능 구현
// ================================================================

/// <summary>
/// ChatGPT API 단일 테스트 함수
/// 사용자로부터 질문을 받아 ChatGPT에게 전달하고 답변을 출력합니다.
/// </summary>
async Task TestChatGpt()
{
    Console.WriteLine("\n=== ChatGPT API 테스트 ===");
    Console.Write("질문을 입력하세요: ");
    var question = Console.ReadLine();
    
    // 입력 유효성 검사
    if (string.IsNullOrWhiteSpace(question))
    {
        Console.WriteLine("질문을 입력해주세요.");
        return;
    }

    Console.WriteLine("ChatGPT에게 질문하는 중...");
    
    // ChatGPT API 호출
    var response = await apiClient.CallChatGptAsync(question);
    
    // 결과 출력
    if (response.Success)
    {
        Console.WriteLine($"\n답변: {response.Answer}");
    }
    else
    {
        Console.WriteLine($"\n오류: {response.Error}");
    }
}

/// <summary>
/// Google Gemini API 단일 테스트 함수
/// 사용자로부터 질문을 받아 Gemini에게 전달하고 답변을 출력합니다.
/// </summary>
async Task TestGemini()
{
    Console.WriteLine("\n=== Gemini API 테스트 ===");
    Console.Write("질문을 입력하세요: ");
    var question = Console.ReadLine();
    
    // 입력 유효성 검사
    if (string.IsNullOrWhiteSpace(question))
    {
        Console.WriteLine("질문을 입력해주세요.");
        return;
    }

    Console.WriteLine("Gemini에게 질문하는 중...");
    
    // Gemini API 호출
    var response = await apiClient.CallGeminiAsync(question);
    
    // 결과 출력
    if (response.Success)
    {
        Console.WriteLine($"\n답변: {response.Answer}");
    }
    else
    {
        Console.WriteLine($"\n오류: {response.Error}");
    }
}

/// <summary>
/// 대화형 테스트 함수
/// 사용자와 AI 간의 연속적인 대화를 처리합니다.
/// </summary>
/// <param name="aiType">AI 타입 ("ChatGPT" 또는 "Gemini")</param>
async Task InteractiveChat(string aiType)
{
    Console.WriteLine($"\n=== {aiType} 대화형 테스트 ===");
    Console.WriteLine("대화를 시작합니다. 'quit' 또는 'exit'을 입력하면 종료됩니다.");
    Console.WriteLine();

    // 대화 루프
    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();
        
        // 빈 입력 무시
        if (string.IsNullOrWhiteSpace(input))
            continue;
            
        // 종료 명령 확인
        if (input.ToLower() is "quit" or "exit")
        {
            Console.WriteLine("대화를 종료합니다.");
            break;
        }

        Console.WriteLine($"{aiType}: 답변을 생성하는 중...");
        
        // AI 타입에 따른 API 호출
        var response = aiType == "ChatGPT" 
            ? await apiClient.CallChatGptAsync(input)
            : await apiClient.CallGeminiAsync(input);
        
        // 응답 출력
        if (response.Success)
        {
            Console.WriteLine($"{aiType}: {response.Answer}");
        }
        else
        {
            Console.WriteLine($"{aiType} 오류: {response.Error}");
        }
        
        Console.WriteLine();    // 대화 간 구분을 위한 빈 줄
    }
}
