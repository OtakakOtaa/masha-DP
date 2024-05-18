using System;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class CustomerVisual : PollEntity, IUniq
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Color _mainColor = Color.white;
        
        
        public Sprite Sprite => _sprite;
        
        public override string ID => _id;
        public override string Name => _id;
        public Color MainColor => _mainColor;
        public override UniqItemsType Type => UniqItemsType.CustomerVisual;
        
        
#if UNITY_EDITOR
        public void SetID_EDITOR(string id)
        {
            _id = id;
        }
#endif
        
    }
}