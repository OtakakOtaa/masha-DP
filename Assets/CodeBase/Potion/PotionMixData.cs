using System.Collections.Generic;
using System.Linq;

namespace _CodeBase.Potion
{
    public sealed class PotionMixData
    {
        private readonly Dictionary<string, int> _parts = new();
        
        
        public Dictionary<string, int> Parts => new(_parts);

        
        
        public PotionMixData() { }

        public PotionMixData(PotionMixData refer)
        {
            _parts = new Dictionary<string, int>(refer._parts);
        }
        
        
        public void AddPart(string part, int amount = 1)
        {
            if (!_parts.TryAdd(part, amount))
            {
                _parts[part] += amount;
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
    }
}