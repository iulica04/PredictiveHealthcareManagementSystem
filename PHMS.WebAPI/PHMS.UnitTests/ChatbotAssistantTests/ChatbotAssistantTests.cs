using Application.AI;
using Xunit;
namespace PHMS.UnitTests.ChatbotUniTests
{
    public class ChatbotAssistantTests
    {
        private ChatbotAssistant _chatbotAssistant;
        public ChatbotAssistantTests()
        {
            _chatbotAssistant = new ChatbotAssistant();
        }
        [Fact]
        public async Task GetResponse_WithValidInput_ReturnsResponse()
        {
            // Arrange
            var userInput = "Hello, how are you?";
            // Act
            var response = await _chatbotAssistant.GetResponse(userInput);
            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
        }
        [Fact]
        public async Task GetResponse_WithNullInput_ReturnsBadRequest()
        {
            // Arrange
            string userInput = null;
            // Act
            var response = await _chatbotAssistant.GetResponse(userInput);
            // Assert
            Assert.Equal("Invalid input.", response);
        }
        [Fact]
        public async Task GetResponse_WithEmptyInput_ReturnsBadRequest()
        {
            // Arrange
            var userInput = "";
            // Act
            var response = await _chatbotAssistant.GetResponse(userInput);
            // Assert
            Assert.Equal("Invalid input.", response);
        }
    }
}