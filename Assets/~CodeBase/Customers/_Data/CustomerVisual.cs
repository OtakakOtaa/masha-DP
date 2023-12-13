using System;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class CustomerVisual : PollEntity
    {
        [SerializeField] private Sprite _sprite;

        public Sprite Sprite => _sprite;
    }
}