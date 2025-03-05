using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using System.Collections.Concurrent;
using System.Net.Http.Headers;

namespace Swagger.AIChat;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ChatController(
    Kernel _kernel,
    ChatAISetting _chatAISetting,
    IHttpClientFactory _httpClientFactory
    ) : ControllerBase
{
    private static readonly ConcurrentDictionary<Guid, ChatHistory> _dictChatHistory = new();

    /// <summary>
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> SendMessageAsync([FromBody] ChatRequest command)
    {
        PrepareKernelOpenApiWithAuthentication(_kernel);

        var chatMessages = _dictChatHistory.GetOrAdd(command.Guid, new ChatHistory(_chatAISetting.SystemPrompt));
        chatMessages.AddUserMessage(command.Message);

        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
            Temperature = 0.6
        };

        var result = await chatCompletionService.GetChatMessageContentAsync(
           chatMessages,
           executionSettings: openAIPromptExecutionSettings,
           kernel: _kernel
           );
        chatMessages.Add(result);

        var reply = result.Items.OfType<TextContent>().First().Text;

        return Ok(new ChatResponse
        {
            Reply = reply
        });
    }

    private void PrepareKernelOpenApiWithAuthentication(Kernel kernel)
    {
        var accessToken = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        var httpClient = _httpClientFactory.CreateClient(_chatAISetting.HttpClientName);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken);

#pragma warning disable SKEXP0040 // 類型僅供評估之用，可能會在未來更新中變更或移除。抑制此診斷以繼續。
        kernel.ImportPluginFromOpenApiAsync(
            pluginName: _chatAISetting.OpenAPIPluginName,
            uri: new Uri(_chatAISetting.OpenAPIUrl),
            new OpenApiFunctionExecutionParameters
            {
                IgnoreNonCompliantErrors = true,
                EnableDynamicPayload = true,
                HttpClient = httpClient
            }
            ).GetAwaiter().GetResult();
#pragma warning restore SKEXP0040 // 類型僅供評估之用，可能會在未來更新中變更或移除。抑制此診斷以繼續。
    }
}