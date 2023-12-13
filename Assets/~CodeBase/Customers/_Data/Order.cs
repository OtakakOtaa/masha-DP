using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class Order : PollEntity
    {
        [TabGroup("message")]
        [ListDrawerSettings(ShowPaging = true, Expanded = true)]
        [TextArea]
        [SerializeField] private List<string> _message;
        
        [TabGroup("core")]
        [SerializeField] private int _reward;
        
        [TabGroup("core")]
        [SerializeField] private string _item;
        
        public List<string> Message => _message;
        public int Reward => _reward;
        public string Item => _item;
    }
}