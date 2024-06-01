using System;
using System.Linq;
using _CodeBase.Infrastructure;
using _CodeBase.Input;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CodeBase.Garden
{
    [ExecuteAlways]
    public sealed class GardenBugsSurface : AnchoredArea
    {
        private class BugDeltaData
        {
            public float movementDelta;
            public Vector3 direction;
            public float rotationFactor;
            public int rotationDirection;
            public bool returningToAreaFlag;
        }

        [Serializable] public sealed class GardenBugsSurfaceSettings
        {
            [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            [SerializeField] private float _movementCurveTimeFactor = 2;
            [SerializeField] private float _movementCurveSpeedFactor = 1;
            [SerializeField] private float _randomRotationOffsetAfterLeaveOut = 5f;
            [SerializeField] private float _rotationAlignFactor = 6f;
            [SerializeField] private Vector2 _perspectiveScale = new Vector2(0.1f, 0.4f);

            public AnimationCurve MovementCurve => _movementCurve;
            public float MovementCurveTimeFactor => _movementCurveTimeFactor;
            public float MovementCurveSpeedFactor => _movementCurveSpeedFactor;
            public float RandomRotationOffsetAfterLeaveOut => _randomRotationOffsetAfterLeaveOut;
            public float RotationAlignFactor => _rotationAlignFactor;
            public Vector2 PerspectiveScale => _perspectiveScale;
        }
        
        
        [SerializeField] private Transform[] _bugs;
        [SerializeField] private Transform _perspectiveScaleStart;
        [SerializeField] private Transform _perspectiveScaleEnd;


        private GardenBugsSurfaceSettings _bugsSurfaceSettings;
        private bool _activeFlag = false;
        private (Transform bug, BugDeltaData data)[] _bugsHandler;
        
        private void Awake()
        {
            var lifeToken = new CompositeDisposable();

            gameObject.OnEnableAsObservable().Subscribe(_ => _activeFlag = true).AddTo(lifeToken);
            gameObject.OnDisableAsObservable().Subscribe(_ => _activeFlag = false).AddTo(lifeToken);
            GameService.GameUpdate.Subscribe(_ => UpdateBugsMovement()).AddTo(lifeToken);
            gameObject.OnDestroyAsObservable().Subscribe(_ => lifeToken.Dispose());


            _bugsHandler = _bugs.Select(c => (c, new BugDeltaData { direction = c.up, rotationFactor = 1f})).ToArray();
        }


        public void Init(GardenBugsSurfaceSettings surfaceSettings)
        {
            _bugsSurfaceSettings = surfaceSettings;
        }
        
        
#if UNITY_EDITOR

        [ExecuteAlways]
        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_perspectiveScaleStart.position, _perspectiveScaleEnd.position);
            Gizmos.DrawSphere(_perspectiveScaleEnd.position, 0.03f);
            base.OnDrawGizmos();
        }
#endif
        
        
        private void UpdateBugsMovement()
        {
            if(_activeFlag is false || _bugsSurfaceSettings == null) return;
            
            for (var i = 0; i < _bugsHandler.Length; i++)
            {
                var item = _bugsHandler[i];


                var position = item.bug.position;
                var topPerspectivePoint = new Vector2(position.x - ((position.x - _perspectiveScaleEnd.position.x) - (position.x - _perspectiveScaleStart.position.x)), _perspectiveScaleEnd.position.y);
                
                var perspective = ((Vector2)position - topPerspectivePoint).magnitude / ((Vector2)_perspectiveScaleEnd.position - (Vector2)_perspectiveScaleStart.position).magnitude;
                
                item.bug.localScale = Vector3.one * Mathf.Lerp(_bugsSurfaceSettings.PerspectiveScale.x, _bugsSurfaceSettings.PerspectiveScale.y, perspective);
                
                var isPlacedOnSurface = CheckPlaceIntoSurface(position);
                if (isPlacedOnSurface && item.data.returningToAreaFlag) item.data.returningToAreaFlag = false;
                
                item.data.rotationFactor += Time.deltaTime;

                var needSwitchToReturn = isPlacedOnSurface is false && item.data.returningToAreaFlag is false; 
                if (needSwitchToReturn)
                {
                    item.data.direction = Quaternion.Euler(0, 0, Random.Range(0, _bugsSurfaceSettings.RandomRotationOffsetAfterLeaveOut)) * -item.data.direction;
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
                
                item.bug.rotation = Quaternion.Slerp(item.bug.rotation, Quaternion.LookRotation(Vector3.forward, item.data.direction), Time.deltaTime * _bugsSurfaceSettings.RotationAlignFactor);
                
                var translationSpeed = Time.deltaTime * _bugsSurfaceSettings.MovementCurve.Evaluate(item.data.movementDelta / _bugsSurfaceSettings.MovementCurveTimeFactor) * _bugsSurfaceSettings.MovementCurveSpeedFactor;
                item.data.movementDelta = item.data.movementDelta > 1f ? 0f : item.data.movementDelta + Time.deltaTime;
                item.bug.position += new Vector3(item.data.direction.x, item.data.direction.y) * translationSpeed;
            }
        }
    }
}