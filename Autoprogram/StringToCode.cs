public class StringToCode {

    public static Dictionary<string, List<string>> GetFilesDiffs(string text)
    {
        Dictionary<string, List<string>> fileContents = new Dictionary<string, List<string>>();
        string[] lines = text.Split("\n");
        int currentIndex = 0;

        while (currentIndex < lines.Length)
        {
            currentIndex = Utils.NextNonBlankIndex(lines, currentIndex);

            if (lines[currentIndex].StartsWith(Utils.FILE_NAME))
            {
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                string currentFile = lines[currentIndex].Trim();
                currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);

                if (currentIndex < lines.Length && lines[currentIndex].StartsWith(Utils.START_CODE))
                {
                    currentIndex = Utils.NextNonBlankIndex(lines, currentIndex+1);
                    string currentContent = "";
                    while (currentIndex < lines.Length && !lines[currentIndex].StartsWith(Utils.END_CODE))
                    {
                        currentContent += lines[currentIndex] + "\n";
                        currentIndex++;
                    }
                    if (!fileContents.TryGetValue(currentFile, out List<string> diffs)) {
                        diffs = new List<string>();
                        fileContents.Add(currentFile, diffs);
                    }
                    diffs.Add(currentContent.Trim());
                }
            }
            else
            {
                currentIndex++;
            }
        }

        return fileContents;
    }  

    public static void SaveFilesToDisk(Dictionary<string, string> files)
    {
        foreach (var file in files)
        {
            string filePath = file.Key;
            string fileContent = file.Value;

            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, fileContent);
        }
    }
}