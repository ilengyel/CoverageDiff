namespace CoverageDiff
{
    using System;
    using System.Collections.Generic;

    public class Comparer2<T> : Comparer<T>
    {
        private readonly Comparison<T> compareFunction;

        public Comparer2(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }

            compareFunction = comparison;
        }

        public override int Compare(T arg1, T arg2)
        {
            return compareFunction(arg1, arg2);
        }
    }
}
