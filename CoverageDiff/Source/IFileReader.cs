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

    public class SourceLine
    {
        public string File { get; set; }

        public int Line { get; set; }

        public string Context { get; set; }

        public override string ToString() => $"{File}({Line}): {Context}";
    }
}
