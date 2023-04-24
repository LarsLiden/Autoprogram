using System.IO;
using System.Text.RegularExpressions;

public class HistoryUpdater
{
    public static void UpdateHistoryFile(string historyFilePath, string currentTask, string response)
    {
        string commentSection = ExtractCommentSection(response);
        string historyEntry = $"{currentTask}\n{commentSection}\n";

        File.AppendAllText(historyFilePath, historyEntry);
    }

    private static string ExtractCommentSection(string response)
    {
        var match = Regex.Match(response, @"\[COMMENT\](.*?)\[FILE NAME\]", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }
}