using DiffMatchPatch;

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
    public static string? ApplyDiffs(string originalCode, List<string> diffs)
    {
        var patches = GetPatches(diffs);
        var newCode = ApplyPatches(originalCode, patches);
        return newCode;
    }

    private static string ApplyPatches(string originalCode, List<Patch> patches) {
        var codeLines = originalCode.Split("\n").ToList();
        foreach (var patch in patches) {
            codeLines = ApplyPatchToLines(codeLines, patch);
        }
        var newCode = string.Join( "\n", codeLines);
        return newCode;
    }

    public static List<string> ApplyPatchToLines(List<string>codeLines, Patch patch) {
        var curFileLine = FindFirstLine(codeLines, patch);

        for (int i = 0; i < patch.patchLines.Count(); i++)  {
            var curPatchLine = patch.patchLines[i];
            if (curPatchLine.type == PatchType.ADD) {
                if (curFileLine < codeLines.Count()) {
                    codeLines.Insert(curFileLine, curPatchLine.text);
                }
                else {
                    codeLines.Add(curPatchLine.text);
                }
                curFileLine++;
            }
            else if (curPatchLine.type == PatchType.DELETE) {
                if (curPatchLine.text.Trim() != codeLines[curFileLine].Trim()) {
                    throw new Exception("Mismatch!");
                }
                codeLines.RemoveAt(curFileLine);
            }
            else if (curPatchLine.type == PatchType.KEEP) {
                curFileLine++;
            }
        }
        return codeLines;
    }

    private static int FindFirstLine(List<string> codeLines, Patch patch) {
        var preLines = GetPreLines(patch);

        for (int i = 0; i <= codeLines.Count() - preLines.Count(); i++)  
        {  
            bool match = true;  
            for (int j = 0; j < preLines.Count(); j++)  
            {  
                if (codeLines[i + j].Trim() != preLines[j].Trim())  
                {  
                    match = false;  
                    break;  
                }  
            }  
            if (match)  
            {  
                return i;  
            }  
        }  
        return -1;  
    }

    // Get list of unedit lines before the first edit
    private static List<string> GetPreLines(Patch patch) {
        var index = 0;
        var preLines = new List<string>();
        while (index < patch.patchLines.Count) {
            if (patch.patchLines[index].type == PatchType.KEEP) {
                preLines.Add(patch.patchLines[index].text);
            }
            else {
                return preLines;
            }
            index++;
        }
        return preLines;
    }

    private static List<Patch> GetPatches(List<string> diffs) {
        var patches = new List<Patch>();

        foreach (var diff in diffs) {
            var lines = diff.Split('\n');  
            var index = 0;

            while (index < lines.Length)
            {
                if (lines[index].StartsWith("@@"))
                {
                    index = Utils.NextNonBlankIndex(lines, index+1);
                    var patch = new Patch();

                    while (index < lines.Length && !lines[index].StartsWith("@@"))
                    {
                        var currentContent = lines[index];
                        PatchLine? patchLine = null;
                        if (currentContent.StartsWith("+")) {
                            currentContent = currentContent.Replace("+", " ");
                            patchLine = new PatchLine(currentContent, PatchType.ADD);
                        }
                        else if (currentContent.StartsWith("-")) {
                            currentContent = currentContent.Replace("-", " ");
                            patchLine = new PatchLine(currentContent, PatchType.DELETE);
                        }
                        // If not a comment keep
                        else if (currentContent.StartsWith("\\")) {
                            patchLine = new PatchLine(currentContent, PatchType.KEEP);
                        }
                        if (patchLine != null) {
                            patch.Add(patchLine);
                        }
                        index++;
                    }
                    patches.Add(patch);
                }
                if (index < lines.Length && !lines[index].StartsWith("@@")) {
                    index++;
                }
            }
        }
        return patches;
    }

    public static string Filter(string input) {
        string[] lines = input.Split('\n');  
        List<string> linesToKeep = new List<string>();  
        
        foreach (string line in lines)  
        {  
            if (line.StartsWith("@") || line.StartsWith("+") || line.StartsWith("-"))  
            {  
                linesToKeep.Add(Utils.CleanString(line));  
            }  
        }  
        
        return string.Join('\n', linesToKeep);  
    }
}