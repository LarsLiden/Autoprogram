using System.Text;
 using System.IO;

public class CodeToString {
    private string ProjectDirectory { get; set; }  
    private List<string> SourceFilesList { get; set; }  

    public CodeToString(string projectDirectory)  
    {  
        ProjectDirectory = projectDirectory;  
        SourceFilesList = new List<string>();  
    }

    public Dictionary<string, string> GetSourceFilesDictionary(List<string>? extensions = null)
    {
        if (extensions == null)
        {
            extensions = new List<string> { ".cs", ".java", ".c", ".cpp", ".js", ".html", ".css" };
        }
        var sourceFilesWithPathAndCode = new Dictionary<string, string>();
        foreach (var filePath in Directory.GetFiles(ProjectDirectory, "*.*", SearchOption.AllDirectories))
        {
            if (!filePath.Contains("Debug") && !filePath.Contains("debug") &&
                extensions.Any(extension => filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase)))
            { 
                var fileText = File.ReadAllText(filePath);
                fileText = Utils.CleanString(fileText);
                sourceFilesWithPathAndCode.Add(filePath, fileText);
            }
        }
        return sourceFilesWithPathAndCode;
    }

    public string DictionaryToString(Dictionary<string, string> sourceFilesDictionary)
    {
        var sourceFilesWithPathAndCode = new StringBuilder();
        foreach (var file in sourceFilesDictionary)
        {
            var withLineNumbers = AddLineNumbers(file.Value);
            sourceFilesWithPathAndCode.AppendLine($"[File]\n{file.Key}");
            sourceFilesWithPathAndCode.AppendLine($"[Code]\n{withLineNumbers}");
            sourceFilesWithPathAndCode.AppendLine();
        }
        return sourceFilesWithPathAndCode.ToString();
    }

    public static string AddLineNumbers(string input)  
    {  
        StringBuilder output = new StringBuilder();  
        int lineNumber = 1;  

        using (StringReader reader = new StringReader(input))  
        {  
            string line;  
            while ((line = reader.ReadLine()) != null)  
            {  
                output.Append(lineNumber.ToString().PadLeft(4, ' '));  
                output.Append(": ");  
                output.AppendLine(line);  
                lineNumber++;  
            }  
        }  

        return output.ToString();  
    }  

    public Dictionary<string, string> ApplyDiffsToFiles(Dictionary<string, string> originalFiles, Dictionary<string, List<string>> fileDiffDict)
    {
        var updatedFiles = new Dictionary<string, string>();

        // Patch files
        foreach (var file in originalFiles)
        {
            if (fileDiffDict.ContainsKey(file.Key))
            {
                string updatedCode = PatchUtility.ApplyDiffs(file.Key, file.Value, fileDiffDict[file.Key]);
                updatedFiles[file.Key] = updatedCode;
            }
        }

        // Add any new files
        foreach (var item in fileDiffDict) 
        {
            if (!originalFiles.ContainsKey(item.Key)) {
                var diffValue =  item.Value.Single();

                // Sometimes new files are given as a patch, othertimes as just files
                string newFile;
                if (diffValue.StartsWith("@@")) {
                    newFile = PatchUtility.ApplyDiffs(item.Key, "", item.Value);
                }
                else {
                    newFile = diffValue;
                }
                updatedFiles.Add(item.Key, newFile);
            }
        }
        return updatedFiles;
    }
}