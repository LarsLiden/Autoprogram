using Xunit;

namespace Autoprogram.Tests
{
    public class CodeToStringTests
    {
        private string TestDirectory
        { 
            get {
                return Path.GetTempPath() + "Tests\\CodeToStringTests";
            }
        }

        [Fact]
        public void GetSourceFiles_ReturnsCorrectFiles()
        {
            // Arrange
            Directory.CreateDirectory(TestDirectory);

            string csFile = TestDirectory + "\\test.cs";
            File.WriteAllText(csFile, "Console.WriteLine(\"Hello World\");");

            string javaFile = TestDirectory + "\\test.java";
            File.WriteAllText(javaFile, "System.out.println(\"Hello World\");");

            string txtFile = TestDirectory + "\\test.txt";
            File.WriteAllText(txtFile, "This is a txt file.");

            var codeToString = new CodeToString(TestDirectory);

            // Act
            string result = codeToString.GetSourceFiles();

            // Assert
            Assert.Contains(csFile, result);
            Assert.Contains(javaFile, result);
            Assert.DoesNotContain(txtFile, result);

            // Cleanup
            Directory.Delete(TestDirectory, true);
        }

        public static string RemoveLF(string input)
        {
            string output = input.Replace("\n", "").Replace("\r", "");
            return output;
        }

        [Fact]
        public void StringToCode_ReturnsCorrectFiles()
        {
            string content1 = $"System.WriteLine(\"Hello World\");";
            string content2 = $"System.out.println(\"Hello Earth\");\nSystem.out.println(\"Take Me To Your Leader\");\n";
            
            // Arrange
            string input = $"[File]\n{TestDirectory}\\test.cs\n[Code]\n{content1}\n" + 
            $"[File]\n{TestDirectory}\\test.java\n[Code]\n{content2}\n";

            // Act
            Dictionary<string, string> result = StringToCode.GetFiles(input);

            char[] trim = new char[] { '\n', '\r'};
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.cs"));
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.java"));
            Assert.Equal(RemoveLF(content1), RemoveLF(result[$"{TestDirectory}\\test.cs"]));
            Assert.Equal(RemoveLF(content2), RemoveLF(result[$"{TestDirectory}\\test.java"]));
        }
    }
}