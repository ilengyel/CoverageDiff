namespace CoverageDiff
{
    using System.Collections.Generic;

    public interface ISequencePointFilter
    {
        void FilterSequencePoints(string input, string output, IList<SourceLine> points);
    }
}