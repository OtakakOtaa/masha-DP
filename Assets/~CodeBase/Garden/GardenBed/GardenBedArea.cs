using System;
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
        
        [SerializeField] private GardenBedCell[] _cells;
        [SerializeField] private GardenBedUI _ui;
        [Space]
        [SerializeField] private GameObject _normalMap;
        [SerializeField] private GameObject _noWaterMap;
        [SerializeField] private GameObject _noFertilizerMap;
        [SerializeField] private GameObject _bugsAttackedMap;
        [Space]
        [SerializeField] private Vector2 _problemTimerRange;
        [SerializeField] private int _waterRequestChance = 50;
        [SerializeField] private int _fertilizeRequestChance = 15;
        [SerializeField] private int _bugsAttackChance = 35;
        [SerializeField] private Vector2 _growingStartRandomOffsetRange;
        [SerializeField] private bool _needConsedStartRandomOffset = false;

        [Inject] private GameConfigProvider _gameConfigProvider;
        
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
        public bool NeedConsedStartRandomOffset => _needConsedStartRandomOffset;
        public Vector2 GrowingStartRandomOffsetRange => _growingStartRandomOffsetRange;
        public PlantConfig PlantConfig { get; private set; }
        

        
        protected override void OnAwake()
        {
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
                (state: State.NeedWater, chance: _waterRequestChance / 100f),
                (state: State.NeedFertilizers, chance: _fertilizeRequestChance / 100f),
                (state: State.NeedBugResolver, chance: _bugsAttackChance / 100f)
            }.OrderByDescending(c => c.chance).ToArray();

            _gardenBedAreaStates = new Dictionary<State, IGardenBedAreaState>()
            {
                [State.Growing] = new BedAreaGrowingState(this),
                [State.NeedHarvest] = new HarvestBedAreaState(this),
            };
            
            var dis = GameService.GameUpdate.Subscribe(_ => UpdateState());
            gameObject.OnDestroyAsObservable().Subscribe(_ => dis.Dispose());
        }

        public void Init(State startState)
        {
            CurrentState = startState;
            _maps.ForEach(m => m.Value.gameObject.SetActive(false));
            _maps[CurrentState].gameObject.SetActive(true);
            _ui.gameObject.SetActive(true);
            _ui.HideAll();
            ProblemTimer.RunWithDuration(_problemTimerRange.y);
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
                ProblemTimer.RunWithEndPoint(TimeSpan.FromSeconds(Time.time + Random.Range(_problemTimerRange.x, _problemTimerRange.y)));
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