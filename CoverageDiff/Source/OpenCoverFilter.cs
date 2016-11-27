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
            foreach (var module in modules.Where(NotSkipped))
            {
                // TODO: Check whether we need to recalculate summary if we remove modules.
                var files = module.Element("Files").Elements();
                var points = ToSequencePoints(source, files);
                if (points.Count == 0)
                {
                    // module.Remove()
                    module.InsertAttribute(0, "skippedDueTo", "DiffCoverageReport");
                    module.Element("Files").Remove();
                    module.Element("Classes").Remove();
                    continue;
                }

                var classes = module.Element("Classes").Elements();
                foreach (var @class in classes.Where(NotSkipped))
                {
                    var keepClass = false;
                    var methods = @class.Element("Methods").Elements();
                    foreach (var method in methods.Where(NotSkipped))
                    {
                        var keepMethod = false;
                        var sequencePts = method.Element("SequencePoints").Elements();
                        foreach (var sequencePt in sequencePts.ToList())
                        {
                            var seqPt = new SequencePoint(sequencePt.AttrInt("fileid"), sequencePt.AttrInt("sl"));
                            if (points.Contains(seqPt))
                            {
                                keepClass = true;
                                keepMethod = true;
                            }
                            else
                            {
                                sequencePt.Remove();
                            }
                        }

                        var branchPts = method.Element("BranchPoints").Elements();
                        foreach (var branchPt in branchPts.ToList())
                        {
                            var brPt = new SequencePoint(branchPt.AttrInt("fileid"), branchPt.AttrInt("sl"));
                            if (points.Contains(brPt))
                            {
                                keepClass = true;
                                keepMethod = true;
                            }
                            else
                            {
                                branchPt.Remove();
                            }
                        }

                        if (!keepMethod)
                        {
                            method.InsertAttribute(0, "skippedDueTo", "DiffCoverageReport");
                            method.Element("SequencePoints").Remove();
                            method.Element("BranchPoints").Remove();
                        }
                    }

                    if (!keepClass)
                    {
                        @class.InsertAttribute(0, "skippedDueTo", "DiffCoverageReport");
                        @class.Element("Methods").Remove();
                    }
                }
            }

            doc.Save(output);
        }

        private bool NotSkipped(XElement element) => element.Attribute("skippedDueTo") == null;

        private HashSet<SequencePoint> ToSequencePoints(IList<SourceLine> source, IEnumerable<XElement> files)
        {
            var points = new HashSet<SequencePoint>();
            var filesLookup = files
                .Select(f => new
                    {
                        FileId = int.Parse(f.Attribute("uid").Value),
                        File = f.Attribute("fullPath").Value.Replace('\\', '/')
                    }).ToArray();
            foreach (var line in source)
            {
                var file = filesLookup.FirstOrDefault(fl => fl.File.EndsWith(line.File, StringComparison.OrdinalIgnoreCase));
                if (file != null)
                {
                    points.Add(new SequencePoint(file.FileId, line.Line));
                }
            }

            return points;
        }

        private struct SequencePoint
        {
            public SequencePoint(int fileId, int line)
            {
                FileId = fileId;
                Line = line;
            }

            public int FileId { get; }

            public int Line { get; }

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
