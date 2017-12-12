using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI.Representation
{
    public class WeightCache
    {
        private Dictionary<Item[], CacheElement> _cache;

        /// <summary>
        /// Gets a value using index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public CacheElement this[Item[] index] {
            get {
                CacheElement c;
                if (TryGet(index, out c))
                {
                    return c;
                }
                return null;
            }
        }

        public WeightCache(Item[][] itemSets)
        {
            _cache = new Dictionary<Item[], CacheElement>();
            foreach (Item[] itemSet in itemSets)
            {
                _cache.Add(itemSet, new CacheElement());
            }

            CacheElement emptyElement;
            if (TryGet(new Item[0], out emptyElement))
            {
                emptyElement.Marked = false;
            }
        }

        /// <summary>
        /// Marks all sets where a specific item appears in.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        public void MarkItem(Item item)
        {
            foreach (KeyValuePair<Item[], CacheElement> pair in _cache)
            {
                if (pair.Key.Contains(item))
                {
                    pair.Value.Marked = true;
                }
            }
        }

        /// <summary>
        /// Attempts to get a cache element, based on a set of items. The match is found using a linear search.
        /// </summary>
        /// <param name="set">The set of items to look for.</param>
        /// <param name="c">The output variable.</param>
        /// <returns>Returns true if successful, false otherwise.</returns>
        public bool TryGet(Item[] set, out CacheElement c)
        {
            c = null;
            foreach (KeyValuePair<Item[], CacheElement> pair in _cache)
            {
                if (pair.Key.Length != set.Length)
                {
                    continue;
                }
                bool found = true;
                foreach (Item item in set)
                {
                    if (!pair.Key.Contains(item))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    c = pair.Value;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// An element in the weight cache.
    /// </summary>
    public class CacheElement
    {
        public bool Marked = true;
        public float Weight = 0;
        public Node[] Path;
    }
}