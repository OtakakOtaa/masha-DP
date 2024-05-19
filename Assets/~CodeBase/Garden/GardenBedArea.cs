﻿using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Input.InteractiveObjsTypes;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace _CodeBase.Garden
{
    public sealed class GardenBedArea : ObjectKeeper
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
        [SerializeField] private int _bugsChance = 35;


        [Inject] private GameDataProvider _gameDataProvider;
        private TimeSpan _targetProblemPoint;
        private float _startGrowTimePoint;
        private Dictionary<State, GameObject> _maps;
        private (State state, float chance)[] _problemsPool;
        private float _currentProgress;
        private PlantConfig _plantConfig;
        
        public State CurrentState { get; private set; }

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
                (state: State.NeedBugResolver, chance: _bugsChance / 100f)
            }.OrderByDescending(c => c.chance).ToArray();

            var dis = GameService.GameUpdate.Subscribe(_ => UpdateState());
            gameObject.OnDestroyAsObservable().Subscribe(_ => dis.Dispose());
        }

        public void Init()
        {
            CurrentState = State.ReadyToUsingWithoutRestrictions;
            _maps.ForEach(m => m.Value.gameObject.SetActive(false));
            _maps[CurrentState].gameObject.SetActive(true);
        }

        public void UpdateState()
        {
            if (CurrentState is State.Growing)
            {
                foreach (var cell in _cells)
                {
                    cell.UpdateProgress();
                }

                _currentProgress = (float)((Time.time - _startGrowTimePoint) /  _plantConfig.GrowTime);
                _ui.UpdateProgressBar(_currentProgress);
                
                if (_currentProgress < 1f) return;
                SwitchState(State.NeedHarvest);
                return;
            }

            if (CurrentState is State.NeedHarvest)
            {
                if (!_cells.All(c => c.HasPlant is false)) return;
                if (Time.time < _targetProblemPoint.TotalSeconds)
                {
                    SwitchState(State.ReadyToUsing);
                }
                else
                {
                    ExecuteProblem();
                }
                return;
            }

            if (CurrentState is State.ReadyToUsing)
            {
                if(Time.time < _targetProblemPoint.TotalSeconds) return;
                ExecuteProblem();
                return;
            }
        }
        
        public override void ProcessInteractivity()
        {
            if (CurrentState is State.ReadyToUsing or State.ReadyToUsingWithoutRestrictions)
            {
                SwitchState(State.Growing);
                return;
            }

            if (CurrentState is State.NeedFertilizers or State.NeedWater or State.NeedBugResolver)
            {
                if (_inputManager.GameplayCursor.Item!.TryGetComponent<GardenTool>(out var tool) && tool.Resolve == CurrentState)
                {
                    SwitchState(State.ReadyToUsing);
                }
                return;
            }
        }

        private void ExecuteProblem()
        {
            var randomValue = Random.Range(0f, 1f);
            
            var totalWeight = 0f;
            foreach (var problem in _problemsPool)
            {
                totalWeight += problem.chance;
                if(totalWeight >= randomValue) SwitchState(problem.state);
            }
        }

        private void SwitchState(State newState)
        {
            _ui.gameObject.SetActive(false);
            _currentProgress = 0f;
            
            if (CurrentState is State.NeedWater or State.NeedFertilizers or State.NeedBugResolver)
            {
                _targetProblemPoint = TimeSpan.FromSeconds(Time.time + Random.Range(_problemTimerRange.x, _problemTimerRange.y));
            }

            if (newState == State.NeedHarvest) _currentProgress = 1;
            
            if (newState == State.Growing)
            {
                _plantConfig = _gameDataProvider.GetByID<PlantConfig>(_inputManager.GameplayCursor.ItemID);
                _cells.ForEach(c => c.ApplyGrownPlantState(_plantConfig, 0f));
                _startGrowTimePoint = Time.time;
                
                _ui.gameObject.SetActive(true);
            }

            _maps.ForEach(m => m.Value.gameObject.SetActive(false));
            _maps[newState].gameObject.SetActive(true);
            CurrentState = newState;
        }
    }
}