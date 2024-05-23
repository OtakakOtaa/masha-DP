using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CodeBase.Infrastructure
{
    public class AnchoredArea : SomeArea
    {
        [FormerlySerializedAs("_surface")] [SerializeField] private Transform[] _anchors;

#if UNITY_EDITOR

        [ExecuteAlways]
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.gray;
            
            for (var i = 1; i < _anchors.Length; i++)
            {
                Gizmos.DrawLine(_anchors[i - 1].position, _anchors[i].position);
            }
            
            Gizmos.DrawLine(_anchors.First().position, _anchors.Last().position);
        }
#endif
        
        public override bool CheckPlaceIntoSurface(Vector2 point)
        {
            var pointAs3 = (Vector3)point;
            
            var value = 0f;
            Vector3 toBorderVec;
            Vector3 toPointVec;
            
            for (var i = 1; i < _anchors.Length; i++)
            {
                toBorderVec = _anchors[i].position - _anchors[i - 1].position;
                toPointVec = pointAs3 - _anchors[i - 1].position;
                
                value = Vector3.Cross(toBorderVec, toPointVec).z;
                if (value < 0) return false;
            }

            var lastIndex = _anchors.Length - 1;
            
            toBorderVec = _anchors[0].position - _anchors[lastIndex].position;
            toPointVec = pointAs3 - _anchors[lastIndex].position;
            
            value = Vector3.Cross(toBorderVec, toPointVec).z;
            return !(value < 0);
        }
    }
}