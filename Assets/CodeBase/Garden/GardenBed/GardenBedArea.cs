﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _CodeBase.DATA;
using _CodeBase.Garden.Data;
using _CodeBase.Garden.UI;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using CodeBase.Audio;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using Random = UnityEngine.Random;
using Timer = _CodeBase.MainGameplay.Timer;

namespace _CodeBase.Garden.GardenBed
{
    public sealed partial class GardenBedArea : ObjectKeeper, IGardenBedAreaState
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


        [SerializeField] private GardenBedCell[] _cells;
        [SerializeField] private GardenBedUI _ui;
        [Space]
        [SerializeField] private GameObject _normalMap;
        [SerializeField] private GameObject _noWaterMap;
        [SerializeField] private GameObject _noFertilizerMap;
        [SerializeField] private GameObject _bugsAttackedMap;
        [SerializeField] private BoxCollider _allHarvestArea;
        [SerializeField] private ParticleSystem _harvestEffect;
        
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _growAudio;
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _harvestNotificationAudio;
        [ValueDropdown("@AudioServiceSettings.GetAllAudioNames()")]
        [SerializeField] private string _harvestAudio;

        
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private AudioService _audioService;

        private GardenBedAreaSettings _settings; 
        private Dictionary<State, GameObject> _maps;
        private Dictionary<State, IGardenBedAreaState> _gardenBedAreaStates;
        private (State type, float chance)[] _problemsPool;

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
        public ParticleSystem HarvestEffect => _harvestEffect;
        public AudioService AudioService => _audioService;

        public string GrowAudio => _growAudio;
        public string HarvestAudio => _harvestAudio;
        public string HarvestNotificationAudio => _harvestNotificationAudio;

        
        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Click, InputManager.InputAction.SomeItemDropped);
            _harvestEffect.Stop();
        }

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

            _problemsPool = _settings.ProblemChanceBrowser
                .Select(i => (type: i.Key, chance: i.Value / 100f))
                .OrderByDescending(c => c.chance)
                .ToArray();
            
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

            _allHarvestArea.enabled = settings.QuickHarvestFlag;
            
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
            
            if(inputAction is InputManager.InputAction.SomeItemDropped)
            {
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
                
                SwitchState(problem.type);
                return;
            }
        }

        public void SwitchState(State newState)
        {
            _ui.HideAll();
            HarvestEffect.Stop();


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