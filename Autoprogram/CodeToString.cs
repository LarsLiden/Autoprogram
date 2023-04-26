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
                fileText = Utils.SubLinefeeds(fileText);
                sourceFilesWithPathAndCode.Add(filePath, fileText);
            }
        }
        return sourceFilesWithPathAndCode;
    }
}