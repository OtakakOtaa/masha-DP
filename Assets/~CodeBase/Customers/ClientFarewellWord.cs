using System;
using UnityEngine;

namespace _CodeBase.Customers
{
    [Serializable] public class ClientFarewellWord : PollEntity, IUniq
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
    }
}