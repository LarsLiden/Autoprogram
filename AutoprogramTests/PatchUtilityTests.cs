using System.IO;
using Xunit;

namespace Autoprogram.Tests
{
    public class PatchUtilityTests
    {
        [Fact]
        public void PatchUtility_PatchUpdatedCode()
        {
            var rootProjectDirectory = Directory.GetCurrentDirectory();
            rootProjectDirectory = System.IO.Directory.GetParent(rootProjectDirectory).Parent.Parent.FullName;

            var originalFile = Utils.ReadFile($"{rootProjectDirectory}//TestData//Program.cs_ORIGINAL");
            var updatedFile = Utils.ReadFile($"{rootProjectDirectory}//TestData//Program.cs_UPDATED");
            var originalLines = originalFile.Split("\n").ToList();
            var updatedLines = updatedFile.Split("\n").ToList();
            var result = PatchUtility.RemoveMissingLines(originalLines, updatedLines);
        }
    }
}