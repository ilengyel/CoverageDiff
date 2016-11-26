namespace CoverageDiff
{
    using System;
    using CommandLine;

    public class DiffCommand
    {
        [Option('c', "coverage", Required = true, HelpText = "OpenCover tool output as an XML file")]
        public string CoverageFile { get; set; }

        [Option('d', "diff", Required = true, HelpText = "Unified format patch file")]
        public string DiffFile { get; set; }

        [Option('o', "output", HelpText = "Output file for mutated coverage xml file.")]
        public string OutputFile { get; set; }

        public int Run()
        {
            try
            {
                // Currently only unified format is supported.
                var points = new UnifiedDiffFileReader().ReadSequencePoints(DiffFile);
                Console.WriteLine("Points:\n" + string.Join("\n", points));

                // Currently only OpenCover is supported.
                var filter = new OpenCoverFilter();
                filter.FilterSequencePoints(CoverageFile, OutputFile, points);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
