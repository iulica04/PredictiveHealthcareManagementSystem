using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class ChatbotAssistant
{
    private const string OpenAiApiKey = "open_AI_key";
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiKey;

    public ChatbotAssistant()
    {

        _apiKey = OpenAiApiKey;

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new Exception("OpenAI API Key is not set.");
        }
    }

    public async Task<string> GetResponse(string userInput)
    {
        var url = "https://api.openai.com/v1/chat/completions";

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
            // Trimiterea cererii fără retry
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);

            return jsonResponse.RootElement
                               .GetProperty("choices")[0]
                               .GetProperty("message")
                               .GetProperty("content")
                               .GetString();
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
