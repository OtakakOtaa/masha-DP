using System;
using _CodeBase.DATA;
using UnityEngine;

namespace _CodeBase.Customers
{
    [Serializable] public class CustomerFarewellWord : PollEntity, IUniq
    {
        [SerializeField] private string _id;
        [TextArea]
        [SerializeField] private string _mess;
        [SerializeField] private bool _isGoodMes;
        
        
        public override string ID => _id;
        public override string Name => string.Empty;
        public string Mess => _mess;
        public bool IsGoodMes => _isGoodMes;
        public override UniqItemsType Type => UniqItemsType.CustomerFarewellWord; 
        public override Sprite Sprite => null;
        
        
#if UNITY_EDITOR
        public void SetID_EDITOR(string id)
        {
            _id = id;
        }
#endif
    }
}