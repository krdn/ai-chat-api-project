using TestClient.Services;

Console.WriteLine("=== AI Chat API 테스트 클라이언트 ===");
Console.WriteLine("1. ChatGPT API 테스트");
Console.WriteLine("2. Gemini API 테스트");
Console.WriteLine("3. 대화형 테스트 (ChatGPT)");
Console.WriteLine("4. 대화형 테스트 (Gemini)");
Console.WriteLine("5. 종료");
Console.WriteLine();

var apiClient = new ApiClient();

while (true)
{
    Console.Write("선택하세요 (1-5): ");
    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await TestChatGpt();
            break;
        case "2":
            await TestGemini();
            break;
        case "3":
            await InteractiveChat("ChatGPT");
            break;
        case "4":
            await InteractiveChat("Gemini");
            break;
        case "5":
            Console.WriteLine("프로그램을 종료합니다.");
            apiClient.Dispose();
            return;
        default:
            Console.WriteLine("잘못된 선택입니다. 다시 선택해주세요.");
            break;
    }
    
    Console.WriteLine();
}

async Task TestChatGpt()
{
    Console.WriteLine("\n=== ChatGPT API 테스트 ===");
    Console.Write("질문을 입력하세요: ");
    var question = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(question))
    {
        Console.WriteLine("질문을 입력해주세요.");
        return;
    }

    Console.WriteLine("ChatGPT에게 질문하는 중...");
    var response = await apiClient.CallChatGptAsync(question);
    
    if (response.Success)
    {
        Console.WriteLine($"\n답변: {response.Answer}");
    }
    else
    {
        Console.WriteLine($"\n오류: {response.Error}");
    }
}

async Task TestGemini()
{
    Console.WriteLine("\n=== Gemini API 테스트 ===");
    Console.Write("질문을 입력하세요: ");
    var question = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(question))
    {
        Console.WriteLine("질문을 입력해주세요.");
        return;
    }

    Console.WriteLine("Gemini에게 질문하는 중...");
    var response = await apiClient.CallGeminiAsync(question);
    
    if (response.Success)
    {
        Console.WriteLine($"\n답변: {response.Answer}");
    }
    else
    {
        Console.WriteLine($"\n오류: {response.Error}");
    }
}

async Task InteractiveChat(string aiType)
{
    Console.WriteLine($"\n=== {aiType} 대화형 테스트 ===");
    Console.WriteLine("대화를 시작합니다. 'quit' 또는 'exit'을 입력하면 종료됩니다.");
    Console.WriteLine();

    while (true)
    {
        Console.Write("You: ");
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
            continue;
            
        if (input.ToLower() is "quit" or "exit")
        {
            Console.WriteLine("대화를 종료합니다.");
            break;
        }

        Console.WriteLine($"{aiType}: 답변을 생성하는 중...");
        
        var response = aiType == "ChatGPT" 
            ? await apiClient.CallChatGptAsync(input)
            : await apiClient.CallGeminiAsync(input);
        
        if (response.Success)
        {
            Console.WriteLine($"{aiType}: {response.Answer}");
        }
        else
        {
            Console.WriteLine($"{aiType} 오류: {response.Error}");
        }
        
        Console.WriteLine();
    }
}
