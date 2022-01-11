using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Skywalker;

namespace System.Collections.Generic
{
    /// <summary>
    /// Extension methods for Collections.
    /// </summary>
    public static class SkywalkerCollectionExtensions
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        /// <summary>
        /// Adds items to the collection which are not already in the collection.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="items">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns the added items.</returns>
        public static IEnumerable<T> AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            Check.NotNull(source, nameof(source));

            var addedItems = new List<T>();

            foreach (var item in items)
            {
                if (source.Contains(item))
                {
                    continue;
                }

                source.Add(item);
                addedItems.Add(item);
            }

            return addedItems;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to decide if the item is already in the collection</param>
        /// <param name="itemFactory">A factory that returns the item</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, Func<T, bool> predicate, Func<T> itemFactory)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(itemFactory, nameof(itemFactory));

            if (source.Any(predicate))
            {
                return false;
            }

            source.Add(itemFactory());
            return true;
        }

        /// <summary>
        /// Removes all items from the collection those satisfy the given <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <param name="source">The collection</param>
        /// <param name="predicate">The condition to remove the items</param>
        /// <returns>List of removed items</returns>
        public static IList<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            var items = source.Where(predicate).ToList();

            foreach (var item in items)
            {
                source.Remove(item);
            }

            return items;
        }

        public static Y[] ConvertAll<X, Y>(this X[] items, Func<X, Y> converter)
        {
            var results = new List<Y>();

            foreach (var item in items)
            {
                var y = converter(item);

                results.Add(y);
            }

            return results.ToArray();
        }

        public static void ForEach<X>(this IEnumerable<X> items, Action<X> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static T? Find<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.Find(items, predicate);
        }

        public static T[] FindAll<T>(this T[] items, Predicate<T> predicate)
        {
            return Array.FindAll(items, predicate);
        }

        /// <summary>
        ///   Generates a HashCode for the contents for the list. Order of items does not matter.
        /// </summary>
        /// <typeparam name="T">The type of object contained within the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The generated HashCode.</returns>
        public static int GetContentsHashCode<T>(this IList<T> list)
        {
            if (list == null)
            {
                return 0;
            }

            var result = 0;
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    // simply add since order does not matter
                    result += list[i]!.GetHashCode();
                }
            }

            return result;
        }

        /// <summary>
        ///   Determines if two lists are equivalent. Equivalent lists have the same number of items and each item is found within the other regardless of respective position within each.
        /// </summary>
        /// <typeparam name="T">The type of object contained within the list.</typeparam>
        /// <param name="listA">The first list.</param>
        /// <param name="listB">The second list.</param>
        /// <returns><c>True</c> if the two lists are equivalent.</returns>
        public static bool AreEquivalent<T>(this IList<T> listA, IList<T> listB)
        {
            if (listA == null && listB == null)
            {
                return true;
            }

            if (listA == null || listB == null)
            {
                return false;
            }

            if (listA.Count != listB.Count)
            {
                return false;
            }

            // copy contents to another list so that contents can be removed as they are found,
            // in order to consider duplicates
            var listBAvailableContents = listB.ToList();

            // order is not important, just make sure that each entry in A is also found in B
            for (var i = 0; i < listA.Count; i++)
            {
                var found = false;

                for (var j = 0; j < listBAvailableContents.Count; j++)
                {
                    if (Equals(listA[i], listBAvailableContents[j]))
                    {
                        found = true;
                        listBAvailableContents.RemoveAt(j);
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Sort a list by a topological sorting, which consider their  dependencies
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A list of objects to sort</param>
        /// <param name="dependenciesResolver">Function to resolve the dependencies</param>
        /// <returns></returns>
        public static List<T> SortByDependencies<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependenciesResolver) where T : notnull
        {
            /* See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
             *      http://en.wikipedia.org/wiki/Topological_sorting
             */

            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, dependenciesResolver, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="item">Item to resolve</param>
        /// <param name="dependenciesResolver">Function to resolve the dependencies</param>
        /// <param name="sorted">List with the sortet items</param>
        /// <param name="visited">Dictionary with the visited items</param>
        private static void SortByDependenciesVisit<T>(this T item, Func<T, IEnumerable<T>> dependenciesResolver, List<T> sorted, Dictionary<T, bool> visited) where T : notnull
        {
            var alreadyVisited = visited.TryGetValue(item, out bool inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = dependenciesResolver(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, dependenciesResolver, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
