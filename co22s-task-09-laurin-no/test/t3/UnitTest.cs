using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DeviceManager;
using DeviceManager.Data;
using Test.Helpers;
using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using AngleSharp.Html.Dom;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using Test.Helpers;
using DeviceManager.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;

namespace test;

public class UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _db;

    public UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _applicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // ... Configure test services
                });

        _client = _applicationFactory.WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddAuthentication("Test")
                                .AddScheme<AuthenticationSchemeOptions, TestAuthHandlerAdmin>(
                                    "Test", options => { });
                        });
                    })
                    .CreateClient(new WebApplicationFactoryClientOptions
                    {
                        AllowAutoRedirect = false,
                    });

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        string connectionstring = "Data Source=app.db";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite(connectionstring);

        _db = new ApplicationDbContext(optionsBuilder.Options);
    }

    [Fact]
    public async void Checkpoint03_01()
    {
        // Arrange
        var client = _applicationFactory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync("/administration");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("http://localhost/Identity/Account/Login",
            response.Headers.Location.OriginalString);
    }

    [Fact]
    public void Checkpoint03_02()
    {
        var t = typeof(HomeController);
        Assert.True(Attribute.IsDefined(t, typeof(AuthorizeAttribute)));
        Assert.True(Attribute.IsDefined(t, typeof(AreaAttribute)));
        AuthorizeAttribute attr = (AuthorizeAttribute)Attribute.GetCustomAttribute(t, typeof(AuthorizeAttribute));
        Assert.NotNull(attr);
        Assert.Equal("Admins", attr.Roles);
        AreaAttribute attr2 = (AreaAttribute)Attribute.GetCustomAttribute(t, typeof(AreaAttribute));
        Assert.NotNull(attr2);
        Assert.Equal("Administration", attr2.RouteValue);
    }

    [Fact]
    public void Checkpoint03_03()
    {
        var t = typeof(UserDevicesController);
        Assert.True(Attribute.IsDefined(t, typeof(AuthorizeAttribute)));
        Assert.True(Attribute.IsDefined(t, typeof(AreaAttribute)));
        AuthorizeAttribute attr = (AuthorizeAttribute)Attribute.GetCustomAttribute(t, typeof(AuthorizeAttribute));
        Assert.NotNull(attr);
        Assert.Equal("Admins", attr.Roles);
        AreaAttribute attr2 = (AreaAttribute)Attribute.GetCustomAttribute(t, typeof(AreaAttribute));
        Assert.NotNull(attr2);
        Assert.Equal("Administration", attr2.RouteValue);
    }

    [Fact]
    public async Task Checkpoint03_04()
    {
        // Arrange
        var users = new List<string>()
        {
            "admin@task.org",
            "user@task.org",
            "demo@task.org"
        };

        //Act
        var response = await _client.GetAsync("/administration");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Act 2
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var table = content.QuerySelector("table");
        var rows = table.QuerySelectorAll("tr");
        var links = table.QuerySelectorAll("a");
        // _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());

        // Assert 2
        Assert.NotNull(table);
        Assert.Equal(4, rows.Count());
        Assert.Equal(3, links.Count());
        var rowContent = string.Join("\n", rows.Select(r => r.TextContent));
        // _testOutputHelper.WriteLine(rowContent);
        foreach (var item in users)
        {
            Assert.Contains(item, rowContent);
        }
    }

    [Fact]
    public async Task Checkpoint03_05()
    {
        // Arrange
        var userId = "7951bc9f-c769-42b7-a601-9c0a9da6a809";
        var devices = new List<Dictionary<string, string>>()
        {
            new Dictionary<string, string>
            {
                {"Id", "4"},
                {"UserId", $"{userId}"},
                {"Name", $"Tablet device"},
                {"Description", "iPad"},
                {"DateAdded", ""}
            },
            new Dictionary<string, string>
            {
                {"Id", "9"},
                {"UserId", $"{userId}"},
                {"Name", $"A device made by Admin"},
                {"Description", "Very cool!"},
                {"DateAdded", ""}
            }
        };

        //Act
        var response = await _client.GetAsync($"/administration/userdevices?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Act 2
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var table = content.QuerySelector("table");
        var rows = table.QuerySelectorAll("tr");
        var links = table.QuerySelectorAll("a");
        // _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());

        // Assert 2
        Assert.NotNull(table);
        Assert.True(rows.Count() >= devices.Count + 1);
        Assert.True(links.Count() >= devices.Count * 3);
        var rowContent = string.Join("\n", rows.Select(r => r.TextContent));
        // _testOutputHelper.WriteLine(rowContent);
        foreach (var item in devices)
        {
            Assert.Contains(item["Name"], rowContent);
            Assert.Contains(item["Description"], rowContent);
        }
    }

    [Fact]
    public async Task Checkpoint03_06()
    {
        // Arrange
        Random rnd = new Random();
        var userId = "7951bc9f-c769-42b7-a601-9c0a9da6a809";
        var device = new Dictionary<string, string>
            {
                {"UserId", $"{userId}"},
                {"Name", $"a new device-{rnd.Next()}"},
                {"Description", $"with desc-{rnd.Next()}"}
            };
        var editedName = $"name-edited-{rnd.Next()}";
        //Act
        var response = await _client.GetAsync($"/administration/userdevices/create?userId={userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Act 2
        var content = await HtmlHelpers.GetDocumentAsync(response);
        var responsePost = await _client.SendAsync(
                (IHtmlFormElement)content.QuerySelector("form"),
                new Dictionary<string, string>
                {
                    {"Name", device["Name"]},
                    {"Description", device["Description"]}
                });

        // Assert 2
        if (responsePost.StatusCode == HttpStatusCode.Redirect)
        {
            var responseRedirect = await _client.GetAsync($"{responsePost.Headers.Location.OriginalString}");
            var resultPage = await responseRedirect.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responseRedirect.IsSuccessStatusCode);
            Assert.Contains(device["Name"], resultPage);
            Assert.Contains(device["Description"], resultPage);
        }
        else
        {
            var resultPage = await responsePost.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responsePost.IsSuccessStatusCode);
            Assert.Contains(device["Name"], resultPage);
            Assert.Contains(device["Description"], resultPage);
        }

        // Arrange 3
        var dbDevice = await _db.Devices.FirstOrDefaultAsync(d => d.Name == device["Name"]);

        // Assert 3
        Assert.NotNull(dbDevice);

        // Act 4
        var responseGetEdit = await _client.GetAsync($"/administration/userdevices/edit/{dbDevice.Id}?userId={userId}");

        // Assert 4
        Assert.Equal(HttpStatusCode.OK, responseGetEdit.StatusCode);

        // Act 5
        var contentEdit = await HtmlHelpers.GetDocumentAsync(responseGetEdit);
        var readonlyInputs = contentEdit.QuerySelectorAll("input[readonly]");
        var responsePostEdit = await _client.SendAsync(
                (IHtmlFormElement)contentEdit.QuerySelector("form"),
                new Dictionary<string, string>
                {
                    {"Name", editedName},
                    {"Description", device["Description"]}
                });
        if (responsePostEdit.StatusCode == HttpStatusCode.Redirect)
        {
            var responseRedirect = await _client.GetAsync($"{responsePostEdit.Headers.Location.OriginalString}");
            var resultPageEdit = await responseRedirect.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responseRedirect.IsSuccessStatusCode);
            Assert.Contains(editedName, resultPageEdit);
            Assert.Contains(device["Description"], resultPageEdit);
        }
        else
        {
            var resultPageEdit = await responsePostEdit.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responsePostEdit.IsSuccessStatusCode);
            Assert.Contains(editedName, resultPageEdit);
            Assert.Contains(device["Description"], resultPageEdit);
        }

        // Assert 5
        Assert.NotNull(readonlyInputs);
        Assert.True(readonlyInputs.Count() > 0);
        Assert.NotNull(await _db.Devices.FirstOrDefaultAsync(d => d.Name == editedName));

        // Act 6
        var responseGetDelete = await _client.GetAsync($"/administration/userdevices/delete/{dbDevice.Id}?userId={userId}");

        // Assert 6
        Assert.Equal(HttpStatusCode.OK, responseGetDelete.StatusCode);

        // Act 7
        var contentDelete = await HtmlHelpers.GetDocumentAsync(responseGetDelete);
        var responsePostDelete = await _client.SendAsync(
                (IHtmlFormElement)contentDelete.QuerySelector("form"),
                new Dictionary<string, string>
                {
                    {"Id", $"{dbDevice.Id}"},
                    {"UserId", $"{userId}"}
                });
        if (responsePostDelete.StatusCode == HttpStatusCode.Redirect)
        {
            var responseRedirect = await _client.GetAsync($"{responsePostDelete.Headers.Location.OriginalString}");
            var resultPageDelete = await responseRedirect.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responseRedirect.IsSuccessStatusCode);
            Assert.DoesNotContain(editedName, resultPageDelete);
            Assert.DoesNotContain(device["Description"], resultPageDelete);
        }
        else
        {
            var resultPageDelete = await responsePostDelete.Content.ReadAsStringAsync();
            // _testOutputHelper.WriteLine(resultPage);

            Assert.True(responsePostDelete.IsSuccessStatusCode);
            Assert.DoesNotContain(editedName, resultPageDelete);
            Assert.DoesNotContain(device["Description"], resultPageDelete);
        }

        // Assert 7
        Assert.Null(await _db.Devices.FirstOrDefaultAsync(d => d.Name == editedName));
    }
}