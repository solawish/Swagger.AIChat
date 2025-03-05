using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swagger.AIChat;

public class SwaggerUIChatAIMiddleware
{
    private const string EmbeddedFileNamespace = "Swagger.AIChat";

    private readonly SwaggerUIOptions _options;

    private readonly StaticFileMiddleware _staticFileMiddleware;

    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SwaggerUIChatAIMiddleware(
        RequestDelegate next,
        IWebHostEnvironment hostingEnv,
        ILoggerFactory loggerFactory,
        SwaggerUIOptions options)
    {
        _options = options ?? new SwaggerUIOptions();

        _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory, options);

        if (options.JsonSerializerOptions != null)
        {
            _jsonSerializerOptions = options.JsonSerializerOptions;
        }
        else
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,

                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false) }
            };
        }
    }

    public async Task Invoke(HttpContext httpContext)
    {       
        await _staticFileMiddleware.Invoke(httpContext);
    }

    private static StaticFileMiddleware CreateStaticFileMiddleware(
            RequestDelegate next,
            IWebHostEnvironment hostingEnv,
            ILoggerFactory loggerFactory,
            SwaggerUIOptions options)
    {
        var staticFileOptions = new StaticFileOptions
        {
            RequestPath = string.IsNullOrEmpty(options.RoutePrefix) ? string.Empty : $"/{options.RoutePrefix}",
            FileProvider = new EmbeddedFileProvider(typeof(SwaggerUIChatAIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace),
        };

        return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
    }
}
