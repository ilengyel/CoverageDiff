namespace CoverageDiff.Tests.Source
{
    using System.Collections.Generic;

    public static class Extensions
    {
        /// <summary>
        /// Render the specified sequence as a coma separated string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence to join.</param>
        /// <returns>The Joined sequence.</returns>
        public static string Join<T>(this IEnumerable<T> sequence)
            => string.Join(", ", sequence);
    }
}
