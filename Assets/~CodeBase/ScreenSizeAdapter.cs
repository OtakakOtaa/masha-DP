using UnityEngine;

namespace _CodeBase
{
    public sealed class ScreenSizeAdapter : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _ui;
    
    
        private float _nativeSize;
        private RectTransform _canvasRectTransform;
        private Vector2 _canvasNativeSize;

        private void Awake()
        {
            _canvasRectTransform = _ui.GetComponent<RectTransform>();
            _canvasNativeSize = _canvasRectTransform.sizeDelta;
            _nativeSize = _camera.orthographicSize;
        }

        public void Update()
        {
            var diff = (Screen.width / (float)Screen.height) / (C.TargetScreenRes.x / C.TargetScreenRes.y);
            _camera.orthographicSize = _nativeSize * (1 + (1f - diff));

            _canvasRectTransform.sizeDelta = new Vector2(_canvasNativeSize.x, _canvasNativeSize.y);
        }
    }
}
