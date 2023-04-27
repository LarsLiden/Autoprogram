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
        public void GetSourceFilesDictionary_ReturnsCorrectFiles()
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
            var result = codeToString.GetSourceFilesDictionary();

            // Assert
            Assert.Contains(csFile, result.Keys);
            Assert.Contains(javaFile, result.Keys);
            Assert.DoesNotContain(txtFile, result.Keys);

            // Cleanup
            Directory.Delete(TestDirectory, true);
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
            Dictionary<string, List<string>> result = StringToCode.GetFilesDiffs(input);


            char[] trim = new char[] { '\n', '\r'};
            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.cs"));
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.java"));
            Assert.Equal(TestUtil.RemoveLF(content1), TestUtil.RemoveLF(result[$"{TestDirectory}\\test.cs"].FirstOrDefault()));
            Assert.Equal(TestUtil.RemoveLF(content2), TestUtil.RemoveLF(result[$"{TestDirectory}\\test.java"].FirstOrDefault()));
        }

        [Fact]
        public void StringToCode_ReturnsCorrectFiles_WithBlankLines()
        {
            string content1 = $"System.WriteLine(\"Hello World\");";
            string content2 = $"System.out.println(\"Hello Earth\");\nSystem.out.println(\"Take Me To Your Leader\");\n";
            
            // Arrange
            string input = $"\n\n[File]\n{TestDirectory}\\test.cs\n\n[Code]\n{content1}\n\n" + 
            $"[File]\n{TestDirectory}\\test.java\n\n[Code]\n{content2}\n\n";

            // Act
            Dictionary<string, List<string>> result = StringToCode.GetFilesDiffs(input);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.cs"));
            Assert.True(result.ContainsKey($"{TestDirectory}\\test.java"));
            Assert.Equal(TestUtil.RemoveLF(content1), TestUtil.RemoveLF(result[$"{TestDirectory}\\test.cs"].FirstOrDefault()));
            Assert.Equal(TestUtil.RemoveLF(content2), TestUtil.RemoveLF(result[$"{TestDirectory}\\test.java"].FirstOrDefault()));
        }
    }
}