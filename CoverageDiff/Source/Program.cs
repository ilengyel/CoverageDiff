// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CoverageDiff
{
    using System;
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
