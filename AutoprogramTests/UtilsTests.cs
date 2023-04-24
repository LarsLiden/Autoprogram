using System.IO;
using Xunit;

namespace Autoprogram.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void NextNonBlankIndex_ReturnsCorrectIndex()
        {
            // Arrange
            string[] lines = { "Line1", "", "Line2", "  ", "Line3", "```Comment", "Line4" };

            // Act
            int index1 = Utils.NextNonBlankIndex(lines, 0);
            int index2 = Utils.NextNonBlankIndex(lines, 1);
            int index3 = Utils.NextNonBlankIndex(lines, 2);
            int index4 = Utils.NextNonBlankIndex(lines, 4, true);

            // Assert
            Assert.Equal(0, index1);
            Assert.Equal(2, index2);
            Assert.Equal(4, index3);
            Assert.Equal(6, index4);
        }
    }
}