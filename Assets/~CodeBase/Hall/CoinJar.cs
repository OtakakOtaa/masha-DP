using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Hall
{
    public sealed class CoinJar : MonoBehaviour
    {
        [SerializeField] private Transform _dropPos;
        [SerializeField] private Vector2 _randomPosOff;
        [SerializeField] private float _randomRotOff;
        [SerializeField] private GameObject _coinPrefab;
        

        [Button(nameof(AddNewCoin))]
        public void AddNewCoin()
        {
            var pos = _dropPos.position +  new Vector3(Random.Range(-_randomPosOff.x, _randomPosOff.x), Random.Range(-_randomPosOff.y, _randomPosOff.y), 0f);
            var rot = Quaternion.identity * Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-_randomRotOff, _randomRotOff)));
            var coin = Instantiate(_coinPrefab, pos, rot, parent: transform.parent);
        }
    }
}