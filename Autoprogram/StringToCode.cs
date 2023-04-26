public class StringToCode {

public static Dictionary<string, string> GetFileDictionary(string text)
    {
        Dictionary<string, string> fileDiffDict = new Dictionary<string, string>();
        string[] lines = text.Split("\n");
        int currentIndex = 0;

        while (currentIndex < lines.Length)
        {
            currentIndex = Utils.NextNonBlankIndex(lines, currentIndex);

            if (currentIndex < lines.Length && lines[currentIndex].StartsWith(Utils.FILE_NAME))
            {
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                string currentFileName = lines[currentIndex].Trim();
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                if (currentIndex < lines.Length && lines[currentIndex].StartsWith(Utils.START_CODE))
                {
                    currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);
                    string? currentContent = null;
                    while (currentIndex < lines.Length && !lines[currentIndex].StartsWith(Utils.END_CODE))
                    {
                        if (currentContent == null) {
                            currentContent = lines[currentIndex] + "\n";
                        }
                        else {
                            currentContent += lines[currentIndex] + "\n";
                        }
                        currentIndex++;
                    }
                    if (currentContent != null) {
                        fileDiffDict.Add(currentFileName, currentContent.Trim());
                    }
                }
            }
            else
            {
                currentIndex++;
            }
        }

        return fileDiffDict;
    }  

    public static Dictionary<string, List<string>> GetFilesDiffs(string text)
    {
        Dictionary<string, List<string>> fileDiffDict = new Dictionary<string, List<string>>();
        string[] lines = text.Split("\n");
        int currentIndex = 0;

        while (currentIndex < lines.Length)
        {
            currentIndex = Utils.NextNonBlankIndex(lines, currentIndex);

            if (currentIndex < lines.Length && lines[currentIndex].StartsWith(Utils.FILE_NAME))
            {
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                string currentFile = lines[currentIndex].Trim();
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                if (currentIndex < lines.Length && lines[currentIndex].StartsWith(Utils.START_CODE))
                {
                    currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);
                    string? currentContent = null;
                    while (currentIndex < lines.Length && !lines[currentIndex].StartsWith(Utils.END_CODE))
                    {
                        if (currentContent == null) {
                            currentContent = lines[currentIndex] + "\n";
                        }
                        else {
                            currentContent += lines[currentIndex] + "\n";
                        }
                        currentIndex++;
                    }
                    if (currentContent != null) {
                        if (!fileDiffDict.TryGetValue(currentFile, out List<string>? diffs)) {
                            diffs = new List<string>();
                            fileDiffDict.Add(currentFile, diffs);
                        }
                        diffs.Add(currentContent.Trim());
                    }
                }
            }
            else
            {
                currentIndex++;
            }
        }

        return fileDiffDict;
    }  

    public static void SaveFilesToDisk(Dictionary<string, string> files)
    {
        foreach (var file in files)
        {
            string filePath = file.Key;
            string fileContent = file.Value;
            string? existingContent = File.Exists(filePath) ? File.ReadAllText(filePath) : null;

            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (existingContent == null || !fileContent.Equals(existingContent))
            {
                File.WriteAllText(filePath, fileContent);
            }

            File.WriteAllText(filePath, fileContent);
        }
    }
}