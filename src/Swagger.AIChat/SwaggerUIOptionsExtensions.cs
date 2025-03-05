using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Swagger.AIChat;

public static class SwaggerUIOptionsExtensions
{
    public static void AddSwaggerChatAIUI(this SwaggerUIOptions options)
    {
        options.EnablePersistAuthorization();

        options.InjectJavascript("/swagger/chat.js");
        options.InjectStylesheet("/swagger/chat.css");
    }

    public static void UseSwaggerChatAIUI(this IApplicationBuilder app, Action<SwaggerUIOptions> setupAction = null)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerUIOptions>>().Value 
            ?? new SwaggerUIOptions();
        setupAction?.Invoke(options);

        app.UseMiddleware<SwaggerUIChatAIMiddleware>(options);
    }
}
