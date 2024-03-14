using System;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class CustomerInfo : PollEntity
    {
        [SerializeField] private string _name;
        public string Name => _name;
    }
}