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
            projectDirectory = System.IO.Directory.GetParent(projectDirectory).FullName;
            var codeToString = new CodeToString(projectDirectory);
            var source = codeToString.GetSourceFiles();

            var path = Directory.GetCurrentDirectory()+@"\\CurrentTask.txt";
            string curTask = "";
            using (StreamReader reader = new StreamReader(path))  
            {  
                curTask = reader.ReadToEnd(); 
            }  
                
            var prompt = $"{curTask}\n{source}";
            Console.WriteLine($"Prompt:\n{prompt}");
            var response = await client.GetResponse(prompt);
            Console.WriteLine($"Response:\n{response}");

            var files = StringToCode.GetFiles(response);
            StringToCode.SaveFilesToDisk(files);
        }
    }
}