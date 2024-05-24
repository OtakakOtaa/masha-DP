using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _CodeBase.Customers._Data
{
    [Serializable] public sealed class Order : PollEntity
    {
        [Space]
        [SerializeField] private string _id;
        
        [ValueDropdown("@MashaEditorUtility.GetAllPotionsID()")]
        [SerializeField] private string _requestedItemID;
        [SerializeField] private int _reward = 30;
        [SerializeField] private bool _needWaitTime = false;
        [SerializeField] private float _timeToReady = -1;


        [TextArea(1, 10)]
        [SerializeField] private string _message;
        [TextArea(1,10)]
        [SerializeField] private string _concreteMessage;
        
        
        public string ConcreteMessage => _concreteMessage;
        public string Message => _message;
        public int Reward => _reward;
        public string RequestedItemID => _requestedItemID;
        public float TimeToReady => _timeToReady;
        public override string ID => _id;
        public override string Name => _id;
        public override UniqItemsType Type => UniqItemsType.Order;
        public bool NeedWaitTime => _needWaitTime;


#if UNITY_EDITOR
        public void SetID_EDITOR(string id)
        {
            _id = id;
        }
#endif
    }
}