using System.Collections.Generic;
using System.Linq;

namespace _CodeBase.Potion
{
    public sealed class PotionMixData
    {
        private readonly Dictionary<string, int> _parts = new();
        
        public override int GetHashCode()
        {
            var sortedParts = _parts.OrderBy(kvp => kvp.Key);

            unchecked
            {
                var hashCode = 17;
                foreach (var kvp in sortedParts)
                {
                    hashCode = hashCode * 31 + kvp.Key.GetHashCode();
                    hashCode = hashCode * 31 + kvp.Value.GetHashCode();
                }

                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode() == GetHashCode();
        }

        public void AddPart(string part)
        {
            if (!_parts.TryAdd(part, 1))
            {
                _parts[part] += 1;
            }
        }

        public void Clear()
        {
            _parts.Clear();
        }

        public bool CheckOfPartialResemblance(PotionMixData comparable)
        {
            foreach (var comparablePart in comparable._parts)
            {
                var isHaveThatPartOnBaseMix = _parts.TryGetValue(comparablePart.Key, out var baseAmount);
                if (isHaveThatPartOnBaseMix is false || baseAmount < comparablePart.Value) return false;
            }
            
            return true;
        }
    }
}