using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CodeBase.Garden
{
    [ExecuteAlways]
    public sealed class GardenBugsSurface : MonoBehaviour
    {
        private class BugDeltaData
        {
            public float movementDelta;
            public Vector3 direction;
            public float rotationFactor;
            public int rotationDirection;
            public bool returningToAreaFlag;
        }
        
        [SerializeField] private Transform[] _surface;
        [SerializeField] private Transform[] _bugs;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _movementCurveFactor;
        [SerializeField] private float _randomRotationOffsetAfterLeaveOut = 0f;
        

        
        private bool _activeFlag = false;
        private (Transform bug, BugDeltaData data)[] _bugsHandler;
        
        private void Awake()
        {
            var lifeToken = new CompositeDisposable();

            gameObject.OnEnableAsObservable().Subscribe(_ => _activeFlag = true).AddTo(lifeToken);
            gameObject.OnDisableAsObservable().Subscribe(_ => _activeFlag = false).AddTo(lifeToken);
            gameObject.UpdateAsObservable().Subscribe(_ => OnUpdate()).AddTo(lifeToken);
            gameObject.OnDestroyAsObservable().Subscribe(_ => lifeToken.Dispose());

            _bugsHandler = _bugs.Select(c => (c, new BugDeltaData { direction = c.up, rotationFactor = 1f})).ToArray();
        }

        private void OnUpdate()
        {
            for (var i = 0; i < _bugsHandler.Length; i++)
            {
                var item = _bugsHandler[i];

                var isPlacedInSurface = CheckPlaceIntoSurface(item.bug.position);
                if (item.data.returningToAreaFlag && isPlacedInSurface) item.data.returningToAreaFlag = false;
                
                item.data.rotationFactor += Time.deltaTime;
                
                if (isPlacedInSurface is false && item.data.returningToAreaFlag is false)
                {
                    item.data.direction = Quaternion.Euler(0, 0, Random.Range(0, _randomRotationOffsetAfterLeaveOut)) * -item.data.direction;
                    item.data.returningToAreaFlag = true;
                }

                if (item.data.returningToAreaFlag)
                {
                    item.data.rotationFactor = 0f;
                }
                else
                {
                    if (item.data.rotationFactor > 1)
                    {
                        item.data.rotationDirection = Random.Range(0, 2) - 1;
                        item.data.rotationFactor = 0f;
                    }

                    item.data.direction = Quaternion.Euler(0, 0, item.data.rotationDirection * item.data.rotationFactor) * item.data.direction;
                }


                var translateSpeed = Time.deltaTime * _movementCurve.Evaluate(item.data.movementDelta) * _movementCurveFactor;

                item.data.movementDelta += Time.deltaTime;
                if (item.data.movementDelta > 1f) item.data.movementDelta = 0f;

                item.bug.position += new Vector3(item.data.direction.x, item.data.direction.y) * translateSpeed;
            }
        }

        private bool CheckPlaceIntoSurface(Vector2 point)
        {
             var pointAs3 = (Vector3)point;
            
            var value = 0f;
            Vector3 toBorderVec;
            Vector3 toPointVec;
            
            for (var i = 1; i < _surface.Length; i++)
            {

                toBorderVec = _surface[i].position - _surface[i - 1].position;
                toPointVec = pointAs3 - _surface[i - 1].position;
                
                value = Vector3.Cross(toBorderVec, toPointVec).z;
                if (value < 0) return false;
            }

            var lastIndex = _surface.Length - 1;
            
            toBorderVec = _surface[0].position - _surface[lastIndex].position;
            toPointVec = pointAs3 - _surface[lastIndex].position;
            
            value = Vector3.Cross(toBorderVec, toPointVec).z;
            return !(value < 0);
        }
    }
}