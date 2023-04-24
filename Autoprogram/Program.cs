using System.Diagnostics;
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
            var sourceFilesDictionary = codeToString.GetSourceFilesDictionary();
            var source = codeToString.DictionaryToString(sourceFilesDictionary);

            var path = Directory.GetCurrentDirectory()+@"\\CurrentTask.txt";
            string curTask = "";
            using (StreamReader reader = new StreamReader(path))  
            {  
                curTask = reader.ReadToEnd(); 
            }  
                
            var prompt = $"{curTask}\n{source}";
            
            var response = await client.GetResponse(prompt);
            Console.WriteLine($"Current Task:\n{curTask}\n\n[COMMENT]:\n{HistoryUpdater.ExtractCommentSection(response)}");

            Console.WriteLine($"Changed:\n{response}");

            var fileDiffDict = StringToCode.GetFilesDiffs(response);
            var files = codeToString.ApplyDiffsToFiles(sourceFilesDictionary, fileDiffDict);

            if (files.Count() == 0) {
                Console.ForegroundColor=ConsoleColor.Red;
                Console.WriteLine("No files to updated.");
                return;
            }
            StringToCode.SaveFilesToDisk(files);

             // Update the history file
            HistoryUpdater.UpdateHistoryFile("history.txt", curTask, response);

            CompileProject(projectDirectory);
/*
            // Initialize CommandLineUtilities with your Github token and create a pull request
            var githubToken = configuration["GithubToken"];
            var commandLineUtilities = new CommandLineUtilities(githubToken);

            // Create a new branch
            string branchName = "new-feature-branch";
            await commandLineUtilities.CreateBranch("repositoryOwner", "repositoryName", branchName);

            // Create a pull request for the new branch
            await commandLineUtilities.CreatePullRequest("repositoryOwner", "repositoryName", branchName, "baseBranch");
*/
        }

        private static void CompileProject(string projectDirectory)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "build /property:WarningLevel=0",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = projectDirectory + "//Autoprogram"
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(10000); // Wait for 60 seconds

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Project compiled successfully.");
            }
            else
            {
                Console.WriteLine("Project compilation failed.");
                Console.WriteLine($"Output: {output}");
                Console.WriteLine($"Error: {error}");
            }
        }
    }
}