using DiffMatchPatch;

public static class PatchUtility
{
    public static string ApplyPatch(string originalCode, string unidiff)
    {
        var dmp = new diff_match_patch();
        var patches = dmp.patch_fromText(unidiff);
        var result = dmp.patch_apply(patches, originalCode);
        return result[0].ToString();
    }
}