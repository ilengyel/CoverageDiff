namespace CoverageDiff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class OpenCoverFilter : ISequencePointFilter
    {
        public void FilterSequencePoints(string input, string output, IList<SourceLine> source)
        {
            // Note if the xml to Linq performance is too bad then need to change to xml reader/writer
            // or do something like: https://lennilobel.wordpress.com/2009/09/02/streaming-into-linq-to-xml-using-c-custom-iterators-and-xmlreader/
            // ... but meh... we all have GBs of ram..
            var doc = XDocument.Load(input);
            var coverage = doc.Element("CoverageSession");
            var summary = coverage.Element("Summary");
            var modules = coverage.Element("Modules").Elements();
            foreach (var module in modules)
            {
                if (module.Attribute("skippedDueTo").Value != null)
                {
                    module.Remove();
                    continue;
                }

                // TODO: Check whether we need to recalculate summary if we remove modules.
                var files = module.Element("Files").Elements();
                var points = ToSequencePoints(source, files);
                if (points.Count == 0)
                {
                    module.Remove();
                    continue;
                }

                Console.WriteLine($"Process module: {module.Element("ModuleName").Value}");
            }

            doc.Save(output);
        }

        private HashSet<SequencePoint> ToSequencePoints(IList<SourceLine> source, IEnumerable<XElement> files)
        {
            var points = new HashSet<SequencePoint>();
            var filesLookup = files
                .Select(f => new { FileId = int.Parse(f.Attribute("uid").Value), File = f.Attribute("fullPath").Value })
                .ToArray();
            foreach (var line in source)
            {
                var file = filesLookup.FirstOrDefault(fl => fl.File.EndsWith(line.File, StringComparison.OrdinalIgnoreCase));
                if (file != null)
                {
                    points.Add(new SequencePoint { FileId = file.FileId, Line = line.Line });
                }
            }

            return points;
        }

        private struct SequencePoint
        {
            public int FileId { get; set; }

            public int Line { get; set; }

            public static bool operator ==(SequencePoint left, SequencePoint right)
                => left.FileId == right.FileId && left.Line == right.Line;

            public static bool operator !=(SequencePoint left, SequencePoint right)
                => left.FileId != right.FileId || left.Line != right.Line;

            public override bool Equals(object obj) => this == (SequencePoint)obj;

            public override int GetHashCode()
            {
                var hash = 23;
                hash = (hash * 31) + FileId.GetHashCode();
                hash = (hash * 31) + Line.GetHashCode();
                return hash;
            }
        }
    }
}
