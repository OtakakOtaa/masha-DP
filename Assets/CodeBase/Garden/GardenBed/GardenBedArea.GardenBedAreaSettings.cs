using System;
using System.Collections.Generic;
using _CodeBase.MainGameplay;
using JetBrains.Annotations;
using UnityEngine;

namespace _CodeBase.Garden.GardenBed
{
    public sealed partial class GardenBedArea
    {
        [Serializable] public sealed class GardenBedAreaSettings
            : ICanBoosted<(Dictionary<State, int> chances, float problemSpanFactor)>, ICanBoosted<bool> 
        {
            [SerializeField] private Vector2 _problemTimerRange = new Vector2(5,10);
            [SerializeField] private int _waterRequestChance = 50;
            [SerializeField] private int _fertilizeRequestChance = 15;
            [SerializeField] private int _bugsAttackChance = 35;
            [SerializeField] private Vector2 _growingStartRandomOffsetRange = new Vector2(0,1);
            [SerializeField] private bool _needConsedStartRandomOffset = true;
            [SerializeField] private State _startState = State.ReadyToUsingWithoutRestrictions;
            
            [SerializeField] private GardenBugsSurface.GardenBugsSurfaceSettings _bugsSurfaceSettings;
            
            [NonSerialized] private Dictionary<State, int> _problemChanceBrowser; 
            [NonSerialized] private float _problemSpanRangeFactor = 1f; 
            [NonSerialized] private bool _quickHarvestFlag = false;
            
            public Vector2 ProblemTimerRange => _problemTimerRange * _problemSpanRangeFactor;
            public bool QuickHarvestFlag => _quickHarvestFlag;
            public Vector2 GrowingStartRandomOffsetRange => _growingStartRandomOffsetRange;
            public bool NeedConsedStartRandomOffset => _needConsedStartRandomOffset;
            public GardenBugsSurface.GardenBugsSurfaceSettings BugsSurfaceSettings => _bugsSurfaceSettings;
            public State StartState => _startState;
            
            public IDictionary<State, int> ProblemChanceBrowser => _problemChanceBrowser ??= CreateBrowser();

            [CanBeNull] public float? GetProblemChance(State type)
            {
                _problemChanceBrowser ??= CreateBrowser();
                
                return _problemChanceBrowser.TryGetValue(type, out var value) ? value / 100f : null; 
            }
            
            
            public void Boost((Dictionary<State, int> chances, float problemSpanFactor) param)
            {
                _problemChanceBrowser = param.chances;
                _problemSpanRangeFactor = param.problemSpanFactor;
            }

            public void Boost(bool param)
            {
                _quickHarvestFlag = param;
            }

            private Dictionary<State, int> CreateBrowser()
            {
                return new Dictionary<State, int>
                {
                    [State.NeedBugResolver] = _bugsAttackChance,
                    [State.NeedFertilizers] = _fertilizeRequestChance,
                    [State.NeedWater] = _waterRequestChance
                };
            }
        }
    }
}