using System.IO;
using Xunit;

namespace Autoprogram.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void CopyProjectToDestination_CopiesFilesCorrectly()
        {
             // Arrange
            string projectDirectory = Path.GetTempPath() + "Tests\\UtilsTests";
            Directory.CreateDirectory(projectDirectory);
            string destinationFolder = Path.Combine(projectDirectory, "Destination");
            Directory.CreateDirectory(destinationFolder);

            string csFile = projectDirectory + "\\test.cs";
            File.WriteAllText(csFile, "Console.WriteLine(\"Hello World\");");

            // Act
            Utils.CopyProjectToDestination(projectDirectory, destinationFolder);

             // Assert
            Assert.True(File.Exists(Path.Combine(destinationFolder, "test.cs")));

             // Cleanup
            Directory.Delete(projectDirectory, true);
        }

        [Fact]
        public void NextNonBlankIndex_ReturnsCorrectIndex()
        {
            // Arrange
            string[] lines = { "Line1", "", "Line2", "  ", "Line3", "```Comment", "Line4" };

            // Act
            int index1 = Utils.NextNonBlankIndex(lines, 0);
            int index2 = Utils.NextNonBlankIndex(lines, 1);
            int index3 = Utils.NextNonBlankIndex(lines, 2);
            int index4 = Utils.NextNonBlankIndex(lines, 4);

            // Assert
            Assert.Equal(0, index1);
            Assert.Equal(2, index2);
            Assert.Equal(4, index3);
            Assert.Equal(6, index4);
        }
    }
}