using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Components.Forms;
using AngleSharp.Dom;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Bunit;
using SignalR_UnitTestingSupportXUnit.Hubs;
using BlazorSignalRApp.Server;
using BlazorSignalRApp.Server.Hubs;
using BlazorSignalRApp.Client.Pages;
using BlazorSignalRApp.Client.Models;
using BlazorSignalRApp.Shared;
using BlazorSignalRApp.Server.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Test.Helpers.Infrastructure;
using Microsoft.AspNetCore.TestHost;

namespace test;

public class UnitTest : IClassFixture<WebApplicationFactoryFixture<Program>>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly TestServer _server;

    private readonly HubUrls _hubUrls;

    public UnitTest(ITestOutputHelper testOutputHelper, WebApplicationFactoryFixture<Program> factoryFixture)
    {
        _testOutputHelper = testOutputHelper;
        _client = factoryFixture.CreateDefaultClient();
        _server = factoryFixture.Server;
        _hubUrls = new HubUrls
        {
            ChatHubUrl = $"{factoryFixture.ServerUrl}/chatter",
            WeatherHubUrl = $"{factoryFixture.ServerUrl}/observations"
        };
    }

    [Fact]
    public async Task Checkpoint01()
    {
        // Arrange
        using var ctx = new TestContext();
        System.Random r = new System.Random();
        var auser = $"user-{r.Next()}";
        var amessage = $"message-{r.Next()}";
        ChatMessage message = new ChatMessage
        {
            User = auser,
            Message = amessage
        };
        ChatMessageNotification notification = new ChatMessageNotification();

        ctx.Services.AddSingleton<HubUrls>(_hubUrls);
        var connection = new HubConnectionBuilder()
            .WithUrl(
                _hubUrls.ChatHubUrl,
                o => o.HttpMessageHandlerFactory = _ => _server.CreateHandler())
            .Build();
        connection.On<ChatMessageNotification>("MessageNotification", msg =>
        {
            notification = msg;
        });

        await connection.StartAsync();

        // Act
        await connection.InvokeAsync("PostMessage", message);
        var response = await _client.GetAsync($"/appdata/messages");

        // Assert
        Assert.Equal(message.User, notification.User);
        Assert.Equal(message.Message, notification.Message);
        Assert.True(response.IsSuccessStatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(auser, content);
        Assert.Contains(amessage, content);
    }
}