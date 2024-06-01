using System;
using System.Linq;
using _CodeBase.Garden.GardenBed;
using _CodeBase.Infrastructure;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CodeBase
{
    [CreateAssetMenu(fileName = nameof(GameplayConfig))]
    public sealed class GameplayConfig : ScriptableObject
    {
        [TabGroup("Main")]
        [SerializeField] private STimeSpan _dayDuration = new(new TimeSpan(0, 0, minutes: 4, 0));
        
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [TabGroup("HAll")]
        [SerializeField] private InspectorFactoredProperty _delayBetweenCustomers = new(1);
        
        [TabGroup("HAll")]
        [SerializeField] private InspectorRandomProperty _firstCustomerEnterDelay = new(new Vector2(0,1));

        [TabGroup("HAll")]
        [SerializeField] private InspectorRandomProperty _bubblePassDelay = new(new Vector2(0,1));

        [TabGroup("HAll")]
        [SerializeField] private InspectorFactoredProperty _textAppearsTemp = new(4);
        
        
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [TabGroup("GARDEN")] 
        [SerializeField] private GardenBedArea.GardenBedAreaSettings[] _areaSettings = new GardenBedArea.GardenBedAreaSettings[5];
        
        
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [TabGroup("POTIONS")] 
        [SerializeField] private Color _cauldronTrashColor = Color.black;
        [TabGroup("POTIONS")] 
        [SerializeField] private Color _mixerTrashColor = Color.black;

        
        
        
        [HideInInspector]
        public static GameplayConfig Instance { get; private set; }
        public void InitInstance() => Instance = this;

        
        public float DayDuration => (float)_dayDuration.Value.TotalSeconds;
        public float GetDelayBetweenCustomers(float i) => _delayBetweenCustomers.GetInterpolatedValue(i);
        public float FirstCustomerEnterDelay => _firstCustomerEnterDelay.Value;
        public float BubblePassDelay => _bubblePassDelay.Value;
        public float GetTextAppearsTemp(float i) => _textAppearsTemp.GetInterpolatedValue(i);
        public GardenBedArea.GardenBedAreaSettings[] AreaSettings => _areaSettings.ToArray();

        public Color MixerTrashColor => _mixerTrashColor;
        public Color CauldronTrashColor => _cauldronTrashColor;
    }
    
    
    
    [Serializable] public sealed class InspectorRandomProperty : IInspectorProperty
    {
        [SerializeField] private Vector2 _range;
        
        public InspectorRandomProperty(Vector2 range)
        {
            _range = range;
        }
        
        public float Value => Random.Range(_range.x, _range.y);
    }

    [Serializable] public sealed class InspectorFactoredProperty : IInspectorProperty
    {
        [SerializeField] private float _value;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 1, 1, 1);

        public InspectorFactoredProperty(float value)
        {
            _value = value;
        }
        
        public float GetInterpolatedValue(float i)
        {
            return _curve.Evaluate(i) * _value;
        } 
        
        public float Value => _value;
    }
    
    public interface IInspectorProperty
    {
        float Value { get; }
    }
}