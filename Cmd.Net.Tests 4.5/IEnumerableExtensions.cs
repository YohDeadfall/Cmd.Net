using System;
using System.Collections.Generic;
using System.Linq;

namespace Cmd.Net.Tests
{
    public static class IEnumerableExtensions
    {
        #region Public Methods

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> source)
        {
            if (source == null)
            { throw new ArgumentNullException("source"); }

            ICollection<T> collection = source as ICollection<T> ?? source.ToArray();

            return PermutationsIterator(collection, collection.Count);
        }

        #endregion

        #region Private Methods

        private static IEnumerable<IEnumerable<T>> PermutationsIterator<T>(IEnumerable<T> source, int count)
        {
            if (count == 1)
            { yield return source; }

            for (int i = 0; i < count; i++)
            {
                foreach (IEnumerable<T> p in PermutationsIterator(source.Take(i).Concat(source.Skip(i + 1)), count - 1))
                { yield return source.Skip(i).Take(1).Concat(p); }
            }
        }

        #endregion
    }
}
