using System.IO;

namespace CoverageDiff.Tests.Source
{
    using System.Collections.Generic;
    using Xunit;

    public class OpenCoverFilterTests
    {
        [Fact]
        public void TestInitial()
        {
            // Arrange
            var points = new List<SourceLine>();
            var target = new OpenCoverFilter();
            var input = Path.Combine("Resources", "OpenCover.xml");
            var output = Path.Combine("OpenCover_filtered.xml");

            // Act
            target.FilterSequencePoints(input, output, points);

            // assert
            // Todo: Be less lazy.
        }
    }
}