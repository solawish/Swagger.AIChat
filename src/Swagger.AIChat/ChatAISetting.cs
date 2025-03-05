namespace Swagger.AIChat;

public class ChatAISetting
{
    /// <summary>
    /// 用於ChatAI的系統提示
    /// </summary>
    public string SystemPrompt { get; set; }

    /// <summary>
    /// HttpClient Name
    /// </summary>
    public string HttpClientName { get; set; } = "ChatAIHttpClient";

    /// <summary>
    /// OpenAPI Plugin Name
    /// </summary>
    /// <example>weather_api</example>
    public string OpenAPIPluginName { get; set; }

    /// <summary>
    /// OpenAPI Document URL
    /// </summary>
    /// <example>https://somewebsite.com/swagger/v1/swagger.json</example>
    public string OpenAPIUrl { get; set; }
}
