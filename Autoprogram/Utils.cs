class Utils {

    public const string COMMENT = "[COMMENT]";
    
    public const string FILE_NAME = "[FILE NAME]";
    
    public const string START_CODE = "[START CODE]";
    public const string END_CODE = "[END CODE]";

    public static string CleanString(string input) {
        return input.Replace("\r\n", "\n");
    }

    // GPT4 has habit of occasionally:
    // 1) Injects commentary with starting with ``` unrelated to the code
    // 2) Putting {no changes} for code that has no changes
    public static int NextNonBlankIndex(string[] lines, int currentIndex) {
        while (string.IsNullOrWhiteSpace(lines[currentIndex]) 
        || string.Equals(lines[currentIndex], "{no changes}", StringComparison.CurrentCultureIgnoreCase)
        || lines[currentIndex].StartsWith("```"))
        {
            currentIndex++;
            if (currentIndex == lines.Length) {
                return currentIndex;
            }
        }
        return currentIndex;
    }

    public static void CopyProjectToDestination(string sourceFolder, string destinationFolder) 
    {
         // Delete destination folder if it exists
        if (Directory.Exists(destinationFolder))
        {
            Directory.Delete(destinationFolder, true);
        }

        var fileInfo = GetAllFilesInDirectory(sourceFolder);

         // Copy files to destination folder
        foreach (var file in fileInfo)
        {
            string localFilePath = file.FullName.Substring(sourceFolder.Length+1);
            string destinationPath = Path.Combine(destinationFolder, localFilePath);
            string destinationDirectory = Path.GetDirectoryName(destinationPath);

             // Exclude files generated by compilation steps
            if (IsSourceFile(file)) {
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                File.Copy(file.FullName, destinationPath, overwrite: true);
            }
        }
    }

    static bool IsSourceFile(FileInfo file) {
        if (file.FullName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                file.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
                file.FullName.Contains("\\bin\\", StringComparison.OrdinalIgnoreCase) ||
                file.FullName.Contains("\\obj\\", StringComparison.OrdinalIgnoreCase) ||
                file.FullName.Contains("\\.git\\", StringComparison.OrdinalIgnoreCase)) 
            {
            return false;
        }
        return true;
    }

    static FileInfo[] GetAllFilesInDirectory(string directoryPath)  
    {  
        DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);  

        // Get all files in the directory  
        FileInfo[] files = directoryInfo.GetFiles();  

        // Get all subdirectories  
        DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();  

        // Get all files in each subdirectory and add them to the files array  
        foreach (DirectoryInfo subDirectory in subDirectories)  
        {  
            FileInfo[] subDirectoryFiles = GetAllFilesInDirectory(subDirectory.FullName);  
            files = MergeFilesArrays(files, subDirectoryFiles);  
        }  

        return files;  
    }  

    static FileInfo[] MergeFilesArrays(FileInfo[] array1, FileInfo[] array2)  
    {  
        FileInfo[] result = new FileInfo[array1.Length + array2.Length];  
        Array.Copy(array1, 0, result, 0, array1.Length);  
        Array.Copy(array2, 0, result, array1.Length, array2.Length);  
        return result;  
    } 

    public static void ColorfulWriteLine(string message, ConsoleColor color)  
    {  
        // Store the current foreground color  
        ConsoleColor originalColor = Console.ForegroundColor;  

        // Set the specified foreground color  
        Console.ForegroundColor = color;  

        // Write the message  
        Console.WriteLine(message);  

        // Restore the original foreground color  
        Console.ForegroundColor = originalColor;  
    }

    public static void CreateFileWithText(string filePath, string text)  
    {  
        // Create directories if missing  
        string dirPath = Path.GetDirectoryName(filePath);  
        if (!Directory.Exists(dirPath))  
        {  
            Directory.CreateDirectory(dirPath);  
        }  
    
        // Create a file with the given text in it  
        File.WriteAllText(filePath, text);  
    }   

    public static bool UserWantsToContinue(string message) {
        Utils.ColorfulWriteLine($"{message}?  (y/n)", ConsoleColor.Yellow);
        var response = Console.ReadLine().ToUpper();  
        return (string.Equals(response, "Y"));
    }
}