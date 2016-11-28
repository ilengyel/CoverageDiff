namespace CoverageDiff
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using static UnifiedDiffFileReader.LineType;

    public class UnifiedDiffFileReader : IFileReader
    {
        private static readonly IOrderedDictionary<LineType, Func<string, bool>> LineMatchers =
            new OrderedDictionary<LineType, Func<string, bool>>
        {
            [FileRemove] = str => str.StartsWith("---"),
            [FileAdd] = str => str.StartsWith("+++"),
            [DiffInfo] = str => str.StartsWith("@@"),
            [LineContext] = str => str.StartsWith(" "),
            [LineAdd] = str => str.StartsWith("+"),
            [LineRemove] = str => str.StartsWith("-"),
        };

        public enum LineType
        {
            Unknown,
            FileRemove,
            FileAdd,
            DiffInfo,
            LineContext,
            LineRemove,
            LineAdd
        }

        public string[] Extensions { get; } = new[] { ".diff", ".patch" };

        public List<SourceLine> ReadSequencePoints(string file)
        {
            var lines = File.ReadAllLines(file);
            var diffFile = "<not set>";
            var diffLine = 0;
            var points = new List<SourceLine>();
            foreach (var line in lines)
            {
                switch (GetLineType(line))
                {
                    case FileAdd:
                        diffFile = line.Substring("+++ b/".Length);
                        diffLine = 0;
                        break;
                    case DiffInfo:
                        // Match like "@@ -11,2 +10,0 @@"
                        var regex = @"\@\@ -(?<oldLine>\d*)(,\d*)* \+(?<newLine>\d*)(,\d*)* \@\@";
                        var match = Regex.Match(line, regex);
                        if (!match.Success)
                        {
                            throw new FormatException($"Did not understand line: {line}.");
                        }

                        diffLine = int.Parse(match.Groups["newLine"].Value);
                        break;
                    case LineContext:
                        diffLine++;
                        break;
                    case LineAdd:
                        points.Add(new SourceLine
                        {
                            File = diffFile,
                            Line = diffLine,
                            Context = line.Substring(1)
                        });
                        diffLine++;
                        break;
                    default:
                        break;
                }
            }

            return points;
        }

        private LineType GetLineType(string line)
            => LineMatchers.FirstOrDefault(m => m.Value(line)).Key;
    }
}
