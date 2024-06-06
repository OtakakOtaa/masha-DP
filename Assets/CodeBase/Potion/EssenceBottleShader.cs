using UnityEngine;

namespace _CodeBase.Potion
{
    public sealed class EssenceBottleShader : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Vector2 _fillRange;
         

        private Material _material;
        private int _fillLevelID;
        private int _mainColorID;
        private int _additiveColorID;

        private void Awake()
        {
            _material = Instantiate(_renderer.material);
            _renderer.material = _material;
            
            _fillLevelID = Shader.PropertyToID("_fill_Level");
            _mainColorID = Shader.PropertyToID("Color_FB35205B");
            _additiveColorID = Shader.PropertyToID("Color_17C6C3C7");
        }


        public void SetProgress(float progress)
        {
            _material.SetFloat(_fillLevelID, Mathf.Lerp(_fillRange.x, _fillRange.y, progress));
        }
        
        public void SetColor(Color color, Color addedColor = default)
        {
            if (addedColor == default)
            {
                addedColor = color * new Color(0.4f, 0.4f, 0.4f);
            }
            
            _material.SetColor(_mainColorID, color);
            _material.SetColor(_additiveColorID, addedColor);
        }
    }
}