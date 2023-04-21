class Utils {

    public static string CleanString(string input) {
        return input.Replace("\r\n", "\n");
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
}