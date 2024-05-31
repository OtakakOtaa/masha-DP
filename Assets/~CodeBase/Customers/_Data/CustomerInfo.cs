using System;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class CustomerInfo : PollEntity, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private string _name;
        
        public override string Name => _name;
        public override string ID => _id;
        public override UniqItemsType Type => UniqItemsType.CustomerInfo;
        public override Sprite Sprite => null;


#if UNITY_EDITOR
        public void SetID_EDITOR(string id)
        {
            _id = id;
        }
#endif
    }
}