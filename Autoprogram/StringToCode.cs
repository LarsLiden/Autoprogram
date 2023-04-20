public class StringToCode {

    public static Dictionary<string, string> GetFilesDiffs(string text)
    {
        Dictionary<string, string> fileContents = new Dictionary<string, string>();
        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        int currentIndex = 0;

        while (currentIndex < lines.Length)
        {
            currentIndex = NextNonBlankIndex(lines, currentIndex);

            if (lines[currentIndex].StartsWith("[File]"))
            {
                currentIndex = NextNonBlankIndex(lines, currentIndex+1);

                string currentFile = lines[currentIndex].Trim();
                currentIndex = NextNonBlankIndex(lines, currentIndex+1);

                if (currentIndex < lines.Length && lines[currentIndex].StartsWith("[Code]"))
                {
                    currentIndex = NextNonBlankIndex(lines, currentIndex+1);
                    string currentContent = "";
                    while (currentIndex < lines.Length && !lines[currentIndex].StartsWith("[File]"))
                    {
                        currentContent += lines[currentIndex] + Environment.NewLine;
                        currentIndex++;
                    }
                    fileContents.Add(currentFile, currentContent.Trim());
                }
            }
            else
            {
                currentIndex++;
            }
        }

        return fileContents;
    }  

    public static int NextNonBlankIndex(string[] lines, int currentIndex) {
        while (string.IsNullOrWhiteSpace(lines[currentIndex]))
        {
            currentIndex++;
            if (currentIndex == lines.Length) {
                return -1;
            }
        }
        return currentIndex;
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