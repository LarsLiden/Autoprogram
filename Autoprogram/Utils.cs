class Utils {

    public const string COMMENT = "[COMMENT]";
    
    public const string FILE_NAME = "[FILE NAME]";
    
    public const string START_CODE = "[START CODE]";
    public const string END_CODE = "[END CODE]";

    public static string CleanString(string input) {
        return input.Replace("\r\n", "\n");
    }

    // GPT4 has habit of occasionally injects commentary with starting with ``` unrelated to the code
    public static int NextNonBlankIndex(string[] lines, int currentIndex, bool removeGPTComments = false) {
        while (string.IsNullOrWhiteSpace(lines[currentIndex]) || (removeGPTComments && lines[currentIndex].StartsWith("```")))
        {
            currentIndex++;
            if (currentIndex == lines.Length) {
                return -1;
            }
        }
        return currentIndex;
    }
}