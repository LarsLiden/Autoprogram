using System.Text;
using System.Text.RegularExpressions;

public class StringToCode {

    public static Dictionary<string, string> GetFiles(string text)
    {
        Dictionary<string, string> fileContents = new Dictionary<string, string>();
        string[] lines = text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        int currentIndex = 0;

        while (currentIndex < lines.Length)
        {
            if (lines[currentIndex].StartsWith("[File]"))
            {
                currentIndex++;
                string currentFile = lines[currentIndex];
                currentIndex++;
                if (currentIndex < lines.Length && lines[currentIndex].StartsWith("[Code]"))
                {
                    currentIndex++;
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
}