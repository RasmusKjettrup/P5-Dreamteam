using System.Collections.Generic;
using System.Linq;

namespace WarehouseAI
{
    public class WeightCache
    {
        private Dictionary<Item[], CacheElement> _cache;
        public CacheElement this[Item[] index] => _cache[index];

        public WeightCache(Item[][] itemSets)
        {
            _cache = new Dictionary<Item[], CacheElement>();
            foreach (Item[] itemSet in itemSets)
            {
                _cache.Add(itemSet, new CacheElement());
            }

            CacheElement emptyElement;
            if (_cache.TryGetValue(new Item[0], out emptyElement))
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
            return _cache.TryGetValue(set, out c);
        }
    }

    public class CacheElement
    {
        public bool Marked = true;
        public float Weight = 0;
    }
}