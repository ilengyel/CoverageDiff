namespace CoverageDiff
{
    using System.Collections.Generic;

    public interface IFileReader
    {
        /// <summary>
        /// Gets the supported file extensions.
        /// </summary>
        string[] Extensions { get; }

        List<SourceLine> ReadSequencePoints(string file);
    }
}
