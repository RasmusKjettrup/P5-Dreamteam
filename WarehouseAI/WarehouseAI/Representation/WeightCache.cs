using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI.Representation
{
    public class WeightCache
    {
        private Dictionary<Item[], CacheElement> _cache;

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

    public class CacheElement
    {
        public bool Marked = true;
        public float Weight = 0;
        public Node[] Path;
    }
}