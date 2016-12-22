using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CoverageDiff.Tests.Source
{
    using Xunit;

    public class UnifiedDiffFileReaderTests
    {
        [Theory]
        [InlineData("Empty.diff", new int[0])]
        [InlineData("RegularSampleNoContext.diff", new[] { 45, 53, 54, 54 })]
        [InlineData("RegularSampleWithContext.diff", new[] { 45, 53, 54, 55, 59, 60, 89 })]
        [InlineData("AddedFile.diff", new[] { 1, 2, 3 })]
        //[InlineData("AddLoolsLikeNewFile.diff", new[] { 5 })]   // TODO: Fix this bug
        public void TestReadFiles(string file, int[] lines)
        {
            // Arrange
            var currPath = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
            var filePath = Path.Combine(currPath, "Resources", file);
            var target = new UnifiedDiffFileReader();

            // Act
            var result = target.ReadSequencePoints(filePath);

            // Assert
            Assert.Equal(lines.Join(), result.Select(sp => sp.Line).Join());
        }
    }
}
