using System;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class Order : PollEntity
    {
        [TextArea]
        [SerializeField] private string _message;
        [TextArea]
        [SerializeField] private string _concreteMessage;
        
        [SerializeField] private int _reward;
        [SerializeField] private string _item;
        [SerializeField] private float _timeToReady;
        
        
        public string ConcreteMessage => _concreteMessage;
        public string Message => _message;
        public int Reward => _reward;
        public string Item => _item;
        public float TimeToReady => _timeToReady;
    }
}