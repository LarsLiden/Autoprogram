using System.IO;
using Xunit;

namespace Autoprogram.Tests
{
    public class PatchUtilityTests
    {
        [Fact]
        public void ApplyPatch_ReturnsCorrectUpdatedCode()
        {
            // Arrange
            string originalCode = "public class HelloWorld {\n" +
                                  "    public static void main(String[] args) {\n" +
                                  "        System.out.println(\"Hello, world!\");\n" +
                                  "    }\n" +
                                  "}\n";

            string unidiff = "@@ -1,5 +1,5 @@\n" +
                             "  public class HelloWorld {\n" +  // Tests extra spaces
                             "     public static void main(String[] args) {\n" +
                             "-        System.out.println(\"Hello, world!\");\n" +
                             "+        System.out.println(\"Hello, Earth!\");\n" +
                             "     }\n" +
                             " }\n";

            string expectedUpdatedCode = "public class HelloWorld {\n" +
                                         "    public static void main(String[] args) {\n" +
                                         "        System.out.println(\"Hello, Earth!\");\n" +
                                         "    }\n" +
                                         "}\n";

            var diffs = new List<string> { unidiff };
            // Act
            string updatedCode = PatchUtility.ApplyDiffs(originalCode, diffs);

            // Assert
            Assert.Equal(TestUtil.RemoveLF(expectedUpdatedCode), TestUtil.RemoveLF(updatedCode), ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
        }

        [Fact]
        public void ApplyPatch_ReturnsCorrectLines()
        {
            var codeLines = new List<string> { "A", "B", "C", "D", "E", "F", "G" };
            var patch = new PatchUtility.Patch();
            patch.Add(new PatchUtility.PatchLine("B", PatchUtility.PatchType.KEEP));
            patch.Add(new PatchUtility.PatchLine("C", PatchUtility.PatchType.DELETE));
            patch.Add(new PatchUtility.PatchLine("D", PatchUtility.PatchType.DELETE));
            patch.Add(new PatchUtility.PatchLine("c", PatchUtility.PatchType.ADD));
            patch.Add(new PatchUtility.PatchLine("d", PatchUtility.PatchType.ADD));
            patch.Add(new PatchUtility.PatchLine("E", PatchUtility.PatchType.KEEP));
            patch.Add(new PatchUtility.PatchLine("F", PatchUtility.PatchType.DELETE));
            patch.Add(new PatchUtility.PatchLine("f", PatchUtility.PatchType.ADD));

            var expectedCodeLines = new List<string> { "A", "B", "c", "d", "E", "f", "G" };

            // Act
            var updatedCodeLines = PatchUtility.ApplyPatchToLines(codeLines, patch);

            // Assert
            Assert.Equal(expectedCodeLines, updatedCodeLines);
        }
    }
}