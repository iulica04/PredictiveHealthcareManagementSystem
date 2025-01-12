using System.Text;
using System.Text.Json;

namespace Application.AI
{
    public class ChatbotAssistant
    {
        private readonly string REQUEST_URL = "https://api.openai.com/v1/chat/completions";
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;

        public ChatbotAssistant(string openAiApiKey = "open_AI_key")
        {
            _apiKey = openAiApiKey;
        }

        public async Task<string> GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "Invalid input.";
            }

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = userInput }
            },
                max_tokens = 150
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var response = await _httpClient.PostAsync(REQUEST_URL, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseContent);

                if (jsonResponse is null)
                {
                    return "No response from the AI model.";
                }

                var responseMessage = jsonResponse
                    .RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()!;
                return responseMessage;
            }
            catch (HttpRequestException ex)
            {
                return $"HTTP Request Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
            }
        }
    }
}
