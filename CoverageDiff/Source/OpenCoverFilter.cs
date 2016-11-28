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
            var modules = coverage.Element("Modules").Elements();
            foreach (var module in modules.Where(IsNotSkipped))
            {
                // TODO: Check whether we need to recalculate summary element if we remove modules, classes, etc.
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
                foreach (var @class in classes.Where(IsNotSkipped))
                {
                    ProcessClass(points, @class);
                }
            }

            doc.Save(output);
        }

        private static void ProcessClass(HashSet<SequencePoint> points, XElement @class)
        {
            var keepClass = false;
            var methods = @class.Element("Methods").Elements();
            foreach (var method in methods.Where(IsNotSkipped))
            {
                var keepMethod = false;
                var sequencePts = method.Element("SequencePoints").Elements();
                FilterPoints(points, sequencePts, ref keepClass, ref keepMethod);

                var branchPts = method.Element("BranchPoints").Elements();
                FilterPoints(points, branchPts, ref keepClass, ref keepMethod);

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

        private static void FilterPoints(HashSet<SequencePoint> points, IEnumerable<XElement> methodPts, ref bool keepClass, ref bool keepMethod)
        {
            foreach (var methodPt in methodPts.ToList())
            {
                var point = new SequencePoint(methodPt.AttrInt("fileid"), methodPt.AttrInt("sl"));
                if (points.Contains(point))
                {
                    keepClass = true;
                    keepMethod = true;
                }
                else
                {
                    methodPt.Remove();
                }
            }
        }

        private static bool IsNotSkipped(XElement element) => element.Attribute("skippedDueTo") == null;

        private static HashSet<SequencePoint> ToSequencePoints(IList<SourceLine> source, IEnumerable<XElement> files)
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
