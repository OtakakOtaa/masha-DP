using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace _CodeBase.Infrastructure
{
    public static class E
    {
        public static TItem GetRandom<TItem>(this IEnumerable<TItem> items)
            => items.ElementAt(new Random().Next(0, items.Count()));
    }
}