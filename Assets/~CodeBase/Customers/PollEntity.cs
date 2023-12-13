using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Customers
{
    [Serializable] public abstract class PollEntity
    {
        [PropertySpace(3,16)]
        [Title("weight")]
        [HideLabel]
        [ProgressBar(0, 1, r: 1, b: 0.2f)]
        [SerializeField] private float _weight;

        public float Weight => _weight;
    }
}