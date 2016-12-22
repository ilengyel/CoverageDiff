namespace CoverageDiff
{
    using CommandLine;

    public static class Program
    {
        /// <summary>
        /// Main method for program
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
            => Parser.Default.ParseArguments<DiffCommand>(args)
                .WithParsed(c => c.Run());
    }
}
