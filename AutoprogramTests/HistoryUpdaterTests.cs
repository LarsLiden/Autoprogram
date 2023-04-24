using System.IO;
using Xunit;

namespace Autoprogram.Tests
{
    public class HistoryUpdaterTests
    {
        [Fact]
        public void UpdateHistoryFile_AppendsCorrectContent()
        {
            // Arrange
            string historyFilePath = Path.GetTempFileName();
            string currentTask = "Current Task: Fix a bug";
            string response = "[COMMENT]\nThis is a comment.\n\n[FILE NAME]\nC:\\test\\file.cs\n[START CODE]\nSome code changes\n[END CODE]";

            string expectedContent = "Current Task: Fix a bug\nThis is a comment.\n\n";
             string expectedContent = "[TASK]\nCurrent Task: Fix a bug\n[COMMENT]\nThis is a comment.\n\n";

            // Act
            HistoryUpdater.UpdateHistoryFile(historyFilePath, currentTask, response);
            string historyFileContent = File.ReadAllText(historyFilePath);

            // Assert
            Assert.Equal(expectedContent, historyFileContent);

            // Cleanup
            File.Delete(historyFilePath);
        }
    }
}