using Microsoft.Extensions.Configuration;

namespace Autoprogram
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var client = new Responder(configuration, "GrindstoneGPT4-32k");

            var projectDirectory = Directory.GetCurrentDirectory();
            var codeToString = new CodeToString(projectDirectory);
            var output = codeToString.GetSourceFiles();
            Console.WriteLine(output);
/*
            var prompt = "Tell me about your mother";
            var response = await client.GetResponse(prompt);
            Console.WriteLine($"Chatbot: {response}");
            */

        }  
    }
}