using Azure;
using Azure.AI.OpenAI;
using static System.Environment;
using Microsoft.Extensions.Configuration;

public class Responder{
    private readonly IConfiguration _config;
    private readonly String _engine;

    public Responder(IConfiguration config, string engine)
    {
        _config = config;
        _engine = engine;
    }

    public async Task<string> GetResponse(string prompt)
    {
        string key = _config["OpenAISettings:Key"];
        string endpoint = _config["OpenAISettings:Endpoint"];

        OpenAIClient client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));

        Console.Write($"Input: {prompt}\n");

        Response<ChatCompletions> responseWithoutStream = await client.GetChatCompletionsAsync(
            _engine,
            new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System, prompt),
                },
                Temperature = (float)0.7,
                MaxTokens = 800,
                NucleusSamplingFactor = (float)0.95,
                FrequencyPenalty = 0,
                PresencePenalty = 0,
            });

        ChatCompletions completions = responseWithoutStream.Value;

        return completions.Choices[0].Message.Content;
    }
}