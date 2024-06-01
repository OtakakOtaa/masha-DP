﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _CodeBase.Garden.Data;
using _CodeBase.Garden.UI;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;
using Timer = _CodeBase.MainGameplay.Timer;

namespace _CodeBase.Garden.GardenBed
{
    public sealed class GardenBedArea : ObjectKeeper, IGardenBedAreaState
    {
        public enum State
        {
            None,
            NeedWater,
            NeedFertilizers,
            NeedBugResolver,
            ReadyToUsing,
            ReadyToUsingWithoutRestrictions,
            Growing,
            NeedHarvest
        }

        [Serializable] public sealed class GardenBedAreaSettings
        {
            [SerializeField] private Vector2 _problemTimerRange = new Vector2(5,10);
            [SerializeField] private int _waterRequestChance = 50;
            [SerializeField] private int _fertilizeRequestChance = 15;
            [SerializeField] private int _bugsAttackChance = 35;
            [SerializeField] private Vector2 _growingStartRandomOffsetRange = new Vector2(0,1);
            [SerializeField] private bool _needConsedStartRandomOffset = true;
            [SerializeField] private State _startState = State.ReadyToUsingWithoutRestrictions;
            
            [SerializeField] private GardenBugsSurface.GardenBugsSurfaceSettings _bugsSurfaceSettings;

            public Vector2 ProblemTimerRange => _problemTimerRange;
            public int WaterRequestChance => _waterRequestChance;
            public int FertilizeRequestChance => _fertilizeRequestChance;
            public int BugsAttackChance => _bugsAttackChance;
            public Vector2 GrowingStartRandomOffsetRange => _growingStartRandomOffsetRange;
            public bool NeedConsedStartRandomOffset => _needConsedStartRandomOffset;
            public GardenBugsSurface.GardenBugsSurfaceSettings BugsSurfaceSettings => _bugsSurfaceSettings;
            public State StartState => _startState;
        }
        
        [SerializeField] private GardenBedCell[] _cells;
        [SerializeField] private GardenBedUI _ui;
        [Space]
        [SerializeField] private GameObject _normalMap;
        [SerializeField] private GameObject _noWaterMap;
        [SerializeField] private GameObject _noFertilizerMap;
        [SerializeField] private GameObject _bugsAttackedMap;
        
        
        
        [Inject] private GameConfigProvider _gameConfigProvider;

        private GardenBedAreaSettings _settings; 
        private Dictionary<State, GameObject> _maps;
        private Dictionary<State, IGardenBedAreaState> _gardenBedAreaStates;
        private (State state, float chance)[] _problemsPool;

        
        public Timer GrowTimer { get; } = new();
        public Timer ProblemTimer { get; } = new();
        public State CurrentState { get; private set; }
        public IGardenBedAreaState StateBehavior { get; private set; }
        public GardenBedCell[] Cells => _cells;
        public GardenBedUI UI => _ui;
        public CancellationToken DestroyCancellationToken => destroyCancellationToken;
        public bool NeedConsedStartRandomOffset => _settings.NeedConsedStartRandomOffset;
        public Vector2 GrowingStartRandomOffsetRange => _settings.GrowingStartRandomOffsetRange;
        public PlantConfig PlantConfig { get; private set; }
        

        
        protected override void OnAwake() { }

        public void Init(GardenBedAreaSettings settings)
        {
            _settings = settings;
            _bugsAttackedMap.GetComponentInChildren<GardenBugsSurface>().Init(settings.BugsSurfaceSettings);
             
            _maps = new Dictionary<State, GameObject>()
            {
                [State.NeedWater] = _noWaterMap,
                [State.NeedFertilizers] = _noFertilizerMap,
                [State.NeedBugResolver] = _bugsAttackedMap,
                [State.ReadyToUsing] = _normalMap,
                [State.ReadyToUsingWithoutRestrictions] = _normalMap,
                [State.Growing] = _normalMap,
                [State.NeedHarvest] = _normalMap,
            }; 

            _problemsPool = new[]
            {
                (state: State.NeedWater, chance: _settings.WaterRequestChance / 100f),
                (state: State.NeedFertilizers, chance: _settings.FertilizeRequestChance / 100f),
                (state: State.NeedBugResolver, chance: _settings.BugsAttackChance / 100f)
            }.OrderByDescending(c => c.chance).ToArray();

            _gardenBedAreaStates = new Dictionary<State, IGardenBedAreaState>()
            {
                [State.Growing] = new BedAreaGrowingState(this),
                [State.NeedHarvest] = new HarvestBedAreaState(this),
            };
            
            var dis = GameService.GameUpdate.Subscribe(_ => UpdateState());
            gameObject.OnDestroyAsObservable().Subscribe(_ => dis.Dispose());
            
            
            CurrentState = settings.StartState;
            _maps.ForEach(m => m.Value.gameObject.SetActive(false));
            _maps[CurrentState].gameObject.SetActive(true);
            _ui.gameObject.SetActive(true);
            _ui.HideAll();
            ProblemTimer.RunWithDuration(_settings.ProblemTimerRange.y);
        }

        
        public void UpdateState()
        {
            if (StateBehavior != null)
            {
                StateBehavior.UpdateState();
                return;
            }
            
            if (CurrentState is State.ReadyToUsing)
            {
                if(ProblemTimer.IsTimeUp is false) return;
                ExecuteProblem();
                return;
            }
        }

        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (StateBehavior != null)
            {
                StateBehavior.ProcessInteractivity(inputAction);
                return;
            }
            
            var isGardenTool = _inputManager.GameplayCursor.HandleItem.TryGetComponent(out GardenTool gardenTool);
            if (CurrentState is State.NeedFertilizers or State.NeedWater or State.NeedBugResolver && isGardenTool && gardenTool.Resolve == CurrentState)
            {
                SwitchState(State.ReadyToUsing);
                return;
            }

            
            var isPantSeed = _inputManager.GameplayCursor.HandleItem.TryGetComponent<SeedDummy>(out var plantDummy);
            if (CurrentState is State.ReadyToUsing or State.ReadyToUsingWithoutRestrictions && isGardenTool is false && isPantSeed)
            {
                PlantConfig = plantDummy.Config;
                SwitchState(State.Growing);
                return;
            }
        }

        public void ExecuteProblem()
        {
            var randomValue = Random.Range(0f, 1f);
            
            var totalWeight = 0f;
            foreach (var problem in _problemsPool)
            {
                totalWeight += problem.chance;
                if (!(totalWeight >= randomValue)) continue;
                
                SwitchState(problem.state);
                return;
            }
        }

        public void SwitchState(State newState)
        {
            _ui.HideAll();


            var nowIsProblemState = CurrentState is State.NeedWater or State.NeedFertilizers or State.NeedBugResolver; 
            if (nowIsProblemState)
            {
                ProblemTimer.RunWithEndPoint(TimeSpan.FromSeconds(Time.time + Random.Range(_settings.ProblemTimerRange.x, _settings.ProblemTimerRange.y)));
            }
            
            StateBehavior = _gardenBedAreaStates.GetValueOrDefault(newState);
            if (StateBehavior != null)
            {
                StateBehavior.SwitchState(newState);
            }
            else
            {
                var isNewStateBeProblem = newState is State.NeedWater or State.NeedFertilizers or State.NeedBugResolver; 
                if (isNewStateBeProblem)
                {
                    _ui.ShowCareRequiredTag();
                }
            }
            
            _maps.ForEach(m => m.Value.gameObject.SetActive(false));
            _maps[newState].gameObject.SetActive(true);
            CurrentState = newState;
         }
    }

    public interface IGardenBedAreaState
    {
        public GardenBedArea.State CurrentState { get; }
        
        void SwitchState(GardenBedArea.State newState);
        void ProcessInteractivity(InputManager.InputAction inputAction);
        void UpdateState();
    }
}