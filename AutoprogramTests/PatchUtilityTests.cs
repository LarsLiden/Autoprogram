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
                             " public class HelloWorld {\n" +
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

            // Act
            string updatedCode = PatchUtility.ApplyPatch(originalCode, unidiff);

            // Assert
            Assert.Equal(TestUtil.RemoveLF(expectedUpdatedCode), TestUtil.RemoveLF(updatedCode), ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
        }
    }
}