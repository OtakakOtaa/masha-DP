using _CodeBase.Garden;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class PlantIngredientBasket : InteractiveObject
    {
        [SerializeField] private PlantConfig _plantConfig;
        [SerializeField] private SpriteRenderer _targetObjSpriteRenderer;

        private bool _activateflag;
        
        private void Start()
        {
            InitSupportedActionsList(InputManager.InputAction.Hold);
        }

        public override void ProcessInteractivity()
        {
            if (!_activateflag)
            {
                if (!GameplayService.Instance.gameplayState.Data.CheckPlantContains(_plantConfig.PlantType)) return;
                GameplayService.Instance.gameplayState.Data.TryRemovePlant(_plantConfig.PlantType);
                _activateflag = true;
            }            
            
            var cursorPos = InputManager.Instance.MousePosInWorld;
            _targetObjSpriteRenderer.transform.position = new Vector3(cursorPos.x, cursorPos.y, _targetObjSpriteRenderer.transform.position.z);
            _targetObjSpriteRenderer.sprite = _plantConfig.Sprite;
            _targetObjSpriteRenderer.gameObject.SetActive(true);
        }

        public override void ProcessEndInteractivity()
        {
            _activateflag = false;
            _targetObjSpriteRenderer.gameObject.SetActive(false);
        }
    }
}