using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Swagger.AIChat;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton(new ChatAISetting 
{
    SystemPrompt = "You are an AI assistant that helps users call APIs.",
    OpenAPIPluginName = "weather_api",
    OpenAPIUrl = "https://localhost:7071/swagger/v1/swagger.json"
} );

var kernelBuilder = builder.Services.AddKernel();
kernelBuilder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-4o-mini",
    modelId: "gpt-4o-mini",
    endpoint: "https://*.openai.azure.com/",
    apiKey: "***"
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.AddSwaggerChatAIUI());
    app.UseSwaggerChatAIUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
