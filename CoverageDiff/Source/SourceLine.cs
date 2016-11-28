namespace CoverageDiff
{
    public class SourceLine
    {
        public string File { get; set; }

        public int Line { get; set; }

        public string Context { get; set; }

        public override string ToString() => $"{File}({Line}): {Context}";
    }
}
