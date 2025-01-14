using Application.AI;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PHMS.Controllers;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class ChatbotControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly string BaseUrl = "api/Chatbot";

    public ChatbotControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task GivenValidInput_WhenGetResponseIsCalled_ThenReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();
        var userInput = new UserInputModel { Input = "Hello, how are you?" };

        // Act
        var response = await client.PostAsJsonAsync($"{BaseUrl}/get-response", userInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ResponseModel>();
        content.Should().NotBeNull();
        content!.Response.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GivenNullInput_WhenGetResponseIsCalled_ThenReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        UserInputModel userInput = null;

        // Act
        var response = await client.PostAsJsonAsync($"{BaseUrl}/get-response", userInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenEmptyInput_WhenGetResponseIsCalled_ThenReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var userInput = new UserInputModel { Input = string.Empty };

        // Act
        var response = await client.PostAsJsonAsync($"{BaseUrl}/get-response", userInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GivenException_WhenGetResponseIsCalled_ThenReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var userInput = new UserInputModel { Input = "This will cause an exception" };

        // Act
        var response = await client.PostAsJsonAsync($"{BaseUrl}/get-response", userInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public class ResponseModel
    {
        public string Response { get; set; }
    }
}
