using System.Text;

public class CodeToString {
    private string ProjectDirectory { get; set; }  
    private List<string> SourceFilesList { get; set; }  

    public CodeToString(string projectDirectory)  
    {  
        ProjectDirectory = projectDirectory;  
        SourceFilesList = new List<string>();  
    }

    public string GetSourceFiles(List<string> extensions = null)  
    {  
            if (extensions == null)  
        {  
            extensions = new List<string> { ".cs", ".java", ".c", ".cpp", ".js", ".html", ".css" };  
        }  
        var sourceFilesWithPathAndCode = new StringBuilder();  
        foreach (var filePath in Directory.GetFiles(ProjectDirectory, "*.*", SearchOption.AllDirectories))  
        {  
            if (!filePath.Contains("Debug") && !filePath.Contains("debug") &&  
                extensions.Any(extension => filePath.EndsWith(extension, StringComparison.OrdinalIgnoreCase)))  
            {  
                sourceFilesWithPathAndCode.AppendLine("==========");  
                sourceFilesWithPathAndCode.AppendLine(filePath);  
                sourceFilesWithPathAndCode.AppendLine("==========");   
                sourceFilesWithPathAndCode.AppendLine(File.ReadAllText(filePath));  
                sourceFilesWithPathAndCode.AppendLine();  
            }  
        }  
        return sourceFilesWithPathAndCode.ToString();  
    }  
}