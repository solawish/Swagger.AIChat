namespace Swagger.AIChat;

public class ChatRequest
{
    /// <summary>
    /// 發送訊息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 對話唯一識別碼
    /// </summary>
    public Guid Guid { get; set; }
}
