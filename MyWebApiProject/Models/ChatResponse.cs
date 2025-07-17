namespace MyWebApiProject.Models;

public class ChatResponse
{
    public string Answer { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Error { get; set; }
}