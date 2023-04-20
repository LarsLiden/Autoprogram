using System.Text;

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
                sourceFilesWithPathAndCode.Add(filePath, File.ReadAllText(filePath));
            }
        }
        return sourceFilesWithPathAndCode;
    }

    public string DictionaryToString(Dictionary<string, string> sourceFilesDictionary)
    {
        var sourceFilesWithPathAndCode = new StringBuilder();
        foreach (var file in sourceFilesDictionary)
        {
            sourceFilesWithPathAndCode.AppendLine($"[File]\n{file.Key}");
            sourceFilesWithPathAndCode.AppendLine($"[Code]\n{file.Value}");
            sourceFilesWithPathAndCode.AppendLine();
        }
        return sourceFilesWithPathAndCode.ToString();
    }

    public Dictionary<string, string> ApplyPatchesToFiles(Dictionary<string, string> originalFiles, Dictionary<string, string> patches)
    {
        var updatedFiles = new Dictionary<string, string>();
        foreach (var file in originalFiles)
        {
            if (patches.ContainsKey(file.Key))
            {
                string updatedCode = PatchUtility.ApplyPatch(file.Value, patches[file.Key]);
                updatedFiles.Add(file.Key, updatedCode);
            }
            else
            {
                updatedFiles.Add(file.Key, file.Value);
            }
        }
        return updatedFiles;
    }
}