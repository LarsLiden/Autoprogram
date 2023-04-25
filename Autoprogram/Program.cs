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

            // Get current project directory
            var projectDirectory = Directory.GetCurrentDirectory();
            projectDirectory = System.IO.Directory.GetParent(projectDirectory).FullName;

            // Make a copy
            string tempProjectDirectory = Path.Combine(Path.GetTempPath(), "TempProject");
            Utils.CopyProjectToDestination(projectDirectory, tempProjectDirectory);

            // Extract code
            var codeToString = new CodeToString(tempProjectDirectory);
            var sourceFilesDictionary = codeToString.GetSourceFilesDictionary();
            var source = codeToString.DictionaryToString(sourceFilesDictionary);

            // Get current task
            var path = Directory.GetCurrentDirectory()+@"\\CurrentTask.txt";
            string curTask = string.Empty;
            using (StreamReader reader = new StreamReader(path))  
            {  
                curTask = reader.ReadToEnd(); 
            }  
            
            if (curTask != "") {
                var prompt = $"{curTask}\n{source}";
                var response = await client.GetResponse(prompt);
                Console.WriteLine($"Current Task:\n{curTask}\n\n[COMMENT]:\n{HistoryUpdater.ExtractCommentSection(response)}");

                Console.WriteLine($"Changed:\n{response}");

                if (!Utils.UserWantsToContinue("Looks good?")) {
                    return;
                }
                var fileDiffDict = StringToCode.GetFilesDiffs(response);
                var files = codeToString.ApplyDiffsToFiles(sourceFilesDictionary, fileDiffDict);

                if (files.Count() == 0) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No files to updated.");
                    return;
                }
                StringToCode.SaveFilesToDisk(files);

                // Update the history file
                HistoryUpdater.UpdateHistoryFile("history.txt", curTask, response);
            }

            CompileProject(tempProjectDirectory, "Autoprogram");
            CompileProject(tempProjectDirectory, "AutoprogramTests");
            ExecuteNUnitTests(tempProjectDirectory, "AutoprogramTests");

            if (!Utils.UserWantsToContinue("Keep Changes?")) {
                Utils.ColorfulWriteLine("Changes abandoned.", ConsoleColor.Blue);
                return;
            }

            // First backup
            string backupProjectDirectory = Path.Combine(Path.GetTempPath(), "TempBackup");
            Utils.CopyProjectToDestination(projectDirectory, backupProjectDirectory);
            Utils.ColorfulWriteLine($"Original code backed up to {backupProjectDirectory}.", ConsoleColor.Blue);

            // Now copy over new code
            Utils.CopyProjectToDestination(tempProjectDirectory, projectDirectory);
            Utils.ColorfulWriteLine("Changes saved.", ConsoleColor.Blue);
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

        private static void ExecuteNUnitTests(string projectDirectory, string project)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "test",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = $"{projectDirectory}\\{project}"
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(10000); // Wait for 60 seconds

            if (process.ExitCode == 0)
            {
                Console.WriteLine("Unit tests completed successfully.");
            }
            else
            {
                var fileName = $"Output//Tests_{project}";
                Utils.ColorfulWriteLine($"Some unit test failed.  See {fileName}", ConsoleColor.Red);
                var text = $"Output: {output}\nError: {error}";
                Utils.CreateFileWithText(fileName, text);
            }
        }

        private static void CompileProject(string projectDirectory, string project)
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
                    WorkingDirectory = $"{projectDirectory}\\{project}"
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit(10000); // Wait for 60 seconds

            if (process.ExitCode == 0)
            {
                Utils.ColorfulWriteLine($"{project} compiled successfully", ConsoleColor.Green);
            }
            else
            {
                var fileName = $"Output//Compile_{project}";
                Utils.ColorfulWriteLine($"{project} compilation failed {projectDirectory}.  See {fileName}", ConsoleColor.Red);
                var text = $"Output: {output}\nError: {error}";
                Utils.CreateFileWithText(fileName, text);
            }
        }
    }
}