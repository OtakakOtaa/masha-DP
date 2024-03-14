using DG.Tweening;
using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class PotionCauldron : MonoBehaviour
    {
        [SerializeField] private GameObject _targetObj;
        [SerializeField] private float _activationDistance;
        [SerializeField] private float _maxDropDuration = 1f;
        [SerializeField] private float _twistStrength;

        private bool _hasSpawnedPrefab;

        private void Update()
        {
            if (_targetObj.activeSelf is false)
            {
                var distance = Vector3.Distance(transform.position, _targetObj.transform.position);

                if (distance <= _activationDistance && !_hasSpawnedPrefab)
                {
                    var copyObj = Instantiate(_targetObj, _targetObj.transform.position, _targetObj.transform.rotation,
                        _targetObj.transform.parent);
                    _hasSpawnedPrefab = true;
                    
                    copyObj.SetActive(true);
                    var startPos = copyObj.transform.position;

                    DOTween.To
                        (
                            setter: v =>
                            {
                                var linPos = Vector3.Lerp(copyObj.transform.position, transform.position, v);
                                copyObj.transform.position = linPos;
                                copyObj.transform.RotateAround(transform.position, Vector3.forward, _twistStrength);
                            },
                            startValue: 0,
                            endValue: 1,
                            duration: _maxDropDuration
                        )
                        .OnComplete(() =>
                        {
                            Destroy(copyObj);
                            _hasSpawnedPrefab = false;
                        });

                    _targetObj.transform.position = new Vector3(100, 0, 0);
                }
            }
        }
    }
}