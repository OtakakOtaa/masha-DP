using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using UnityEngine;
using VContainer;

namespace _CodeBase.Potion
{
    public sealed class EssenceMixer : ObjectKeeper
    {
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private GameplayService _gameplayService;
        
        
        private Dictionary<PotionMixData, string> _mixMap; 
        private PotionMixData _currentPotionMix;
        private string _targetMix;


        protected override void OnAwake()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold, InputManager.InputAction.SomeItemDropped);
        }

        
        
        public void Init()
        {
            _mixMap = new Dictionary<PotionMixData, string>();
            
            var allAccessedEssences = _gameplayService.Data.AllEssences.ToArray();
            var allPotions = _gameConfigProvider.Potions.ToArray();
            
            foreach (var potion in allPotions)
            {
                var essenceComponents = potion.Compound.Where(c => allAccessedEssences.Contains(c.ID)).ToArray();
                if (!(essenceComponents.Length >= 2f)) continue;

                var finalMix = new PotionMixData();
                foreach (var essenceComponent in essenceComponents)
                {
                    for (var i = 0; i < essenceComponent.Amount; i++)
                    {
                        finalMix.AddPart(essenceComponent.ID);
                    }
                }
                
                _mixMap[finalMix] = potion.ID;
            }
        } 
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (inputAction == InputManager.InputAction.SomeItemDropped)
            {
                var essenceBottle = _inputManager.GameplayCursor.HandleItem as EssenceBottle;
                if (essenceBottle == null) return;

                _currentPotionMix ??= new PotionMixData();
                var key = essenceBottle.EssenceID;
            
            
                _currentPotionMix.AddPart(essenceBottle.EssenceID);

                var state = GetCurrentFixState();

                
                _targetMix = _mixMap.GetValueOrDefault(_currentPotionMix);
                
                Debug.Log(state.ToString());    
                return;
            }

            if (inputAction == InputManager.InputAction.Hold)
            {
                transform.position = _inputManager.WorldPosition;
                
                return;
            }
        }


        private int GetCurrentFixState()
        {
            var isHasThatMix = _mixMap.ContainsKey(_currentPotionMix);
            
            if (isHasThatMix) return 1;

            if (_mixMap.Any(finalMix => finalMix.Key.CheckOfPartialResemblance(_currentPotionMix))) return 0;

            return -1;
        }
    }
}