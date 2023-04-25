using System.Text.RegularExpressions;

public class HistoryUpdater
{
    public static void UpdateHistoryFile(string historyFilePath, string currentTask, string response)
    {
        string commentSection = $"[TASK]\n{currentTask}\n[COMMENT]\n{ExtractCommentSection(response)}";
        File.AppendAllText(historyFilePath, commentSection);
    }

    public static string ExtractCommentSection(string response)
    {
        var match = Regex.Match(response, @"\[COMMENT\](.*?)\[FILE NAME\]", RegexOptions.Singleline);
        return match.Success ? match.Groups[1].Value.Trim() : "";
    }
}