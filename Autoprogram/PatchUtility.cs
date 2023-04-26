using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

// Can't apply conventional libraries for UniDiff patching because returned values from GPT is fuzzy.  Need to 
// use a more robust algorithm that can handle mistakes
public static class PatchUtility
{

    public enum PatchType {
        ADD,
        DELETE,
        KEEP
    }

    public class Patch {
        public List<PatchLine> patchLines = new List<PatchLine>();

        public void Add(PatchLine line) {
            patchLines.Add(line);
        }
    }

    public class PatchLine {
        public PatchType type;
        public string  text;

        public PatchLine(string text, PatchType type) {
            this.text = text;
            this.type = type;
        }
    }

    public static int FindFirstString(List<string> lines, string search)  
    {  
        for (int i = 0; i < lines.Count; i++)  
        {  
            if (lines[i].Contains(search, StringComparison.CurrentCultureIgnoreCase))  
            {  
                return i;  
            }  
        }  
    
        return -1; // Return -1 if no match was found  
    }

    public static List<string> GetPrecedingItems(List<string> list, int index, int numProceeding)  
    {  
        int startIndex = Math.Max(index - 1 - numProceeding, 0);  
        int count = index - startIndex;  
    
        return list.GetRange(startIndex, count);  
    }  

    public static int FindSubListEnd(List<string> subList, List<string> list)  
    {  
        int smallLength = subList.Count;  
        int largeLength = list.Count;  

        for (int i = 0; i < largeLength - smallLength + 1; i++)  
        {  
            bool match = true;  

            for (int j = 0; j < smallLength; j++)  
            {  
                if (list[i + j] != subList[j])  
                {  
                    match = false;  
                    break;  
                }  
            }  

            if (match)  
            {  
                return i + smallLength;  
            }  
        }  

        return -1; // Return -1 if no match was found  
    } 

    public static List<string> TruncateList(List<string> list, int index)  
    {  
        if (index < 0)  
        {  
            return new List<string>();  
        }  
    
        int count = Math.Min(index + 1, list.Count);  
        return list.GetRange(0, count);  
    }  

    public static int FindRemainingCode(List<string> updatedLines, List<string> originalLines, int restIndex) {
        var curTestIndex = restIndex;
        while (curTestIndex > 5) {
            var precedingItems = GetPrecedingItems(updatedLines, curTestIndex, 5);
            var subListEnd = FindSubListEnd(precedingItems, originalLines);
            if (subListEnd > 0) {
                return subListEnd;
            }
            curTestIndex--;
        }
        return -1;
    }

    private static string PatchUpdatedCode(string originalFile, string updatedFile) {

        if (updatedFile.Contains("rest of the code",StringComparison.CurrentCultureIgnoreCase)) {
            var originalLines = originalFile.Split("\n").ToList();
            var updatedLines = updatedFile.Split("\n").ToList();

            var restIndex = FindFirstString(updatedLines, "rest of the code");

            var patchPosition = FindRemainingCode(updatedLines, originalLines, restIndex);
            var patchLines = originalLines.GetRange(patchPosition, originalLines.Count-patchPosition);
    
            updatedLines = TruncateList(updatedLines, restIndex-1);
            updatedLines.AddRange(patchLines);
            var updatedList = string.Join("\n", updatedLines);
            return updatedList;
        }
        return updatedFile;
    }
 
    private static string GetDiff(string originalFile, string updatedFile) {
        var diff = InlineDiffBuilder.Diff(originalFile, updatedFile);

        var savedColor = Console.ForegroundColor;
        foreach (var line in diff.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("+ ");
                    break;
                case ChangeType.Deleted:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("- ");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray; // compromise for dark or light background
                    Console.Write("  ");
                    break;
            }

            Console.WriteLine(line.Text);
        }
        Console.ForegroundColor = savedColor;
        return "yes";
    }

    public static Dictionary<string, string> GetDiffDict(Dictionary<string, string> originalFilesDict, Dictionary<string, string> updatedFilesDict) {
        var diffDict = new Dictionary<string,string>();
        foreach (var item in updatedFilesDict) {
            if (originalFilesDict.TryGetValue(item.Key, out string? existingFileContent)) {
                var updatedFileContent = PatchUpdatedCode(existingFileContent, item.Value);
                var diff = GetDiff(existingFileContent, updatedFileContent);
                diffDict.Add(item.Key, diff);
            }
            else {
                // It's a new file
                diffDict.Add(item.Key, item.Value);
            }
        }
        return diffDict;
    }

    public static string ChangeString(Dictionary<string, string> originalFilesDict, Dictionary<string, string> updatedFilesDict) {
        var diffDict = GetDiffDict(originalFilesDict, updatedFilesDict);
        var text = Utils.CodeDictionaryToString(diffDict);
        return text;
    }
}