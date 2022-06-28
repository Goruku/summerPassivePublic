using UnityEngine;
using UnityEngine.EventSystems;

namespace Passive {
    public class Scrollable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IScrollHandler {
        public bool screenSnap;
        public bool rescale;
        public bool dragging;
        private RectTransform _rectTransform;
        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnPointerDown(PointerEventData eventData) {
            dragging = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            dragging = false;
        }

        public void OnScroll(PointerEventData eventData) {
            if (eventData.scrollDelta.y < 0) {
                _rectTransform.localScale = _rectTransform.localScale / 1.2f;
            }
            if (eventData.scrollDelta.y > 0) {
                _rectTransform.localScale =  _rectTransform.localScale * 1.2f;
            }
            if (rescale)
                Rescale();
            if (screenSnap)
                SnapScreenEdge();
        }

        private void SnapScreenEdge() {
            var (sizeX , sizeY) = _rectTransform.sizeDelta;
            var (scaleX, scaleY, scaleZ) = _rectTransform.localScale;
            var (parentSizeX, parentSizeY) = transform.parent.GetComponent<RectTransform>().sizeDelta;

            var edgeX = (sizeX * scaleX) * 0.5f - (parentSizeX * 0.5f);
            var edgeY = (sizeY * scaleY) * 0.5f - (parentSizeY * 0.5f);

            // assume centered
            if (_rectTransform.anchoredPosition.x > edgeX) {
                _rectTransform.anchoredPosition = new Vector2(edgeX, _rectTransform.anchoredPosition.y);
            }
            if (_rectTransform.anchoredPosition.x < -edgeX) {
                _rectTransform.anchoredPosition = new Vector2(-edgeX, _rectTransform.anchoredPosition.y);
            }
            
            if (_rectTransform.anchoredPosition.y > edgeY) {
                _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, edgeY);
            }
            if (_rectTransform.anchoredPosition.y < -edgeY ){
                _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, -edgeY);
            }
        }

        private void Rescale() {
            var parentTransform = transform.parent.GetComponent<RectTransform>();
            if (_rectTransform.sizeDelta.x * _rectTransform.localScale.x < Screen.width) {
                var computedScale = parentTransform.sizeDelta.x / _rectTransform.sizeDelta.x;
                _rectTransform.localScale = new Vector3(computedScale, computedScale, computedScale);
            }
            if (_rectTransform.sizeDelta.y * _rectTransform.localScale.y < Screen.height) {
                var computedScale = parentTransform.sizeDelta.y / _rectTransform.sizeDelta.y;
                _rectTransform.localScale = new Vector3(computedScale, computedScale, computedScale);
            }
        }

        public void OnPointerMove(PointerEventData eventData) {
            if (dragging) {
                _rectTransform.anchoredPosition += eventData.delta;
                if (rescale)
                    Rescale();
                if (screenSnap)
                    SnapScreenEdge();
            }
        }
        
    }

    public static class VectorExtension {
        public static void Deconstruct(this Vector2 value, out float x, out float y)
        {
            x = value.x;
            y = value.y;
        }
        
        public static void Deconstruct(this Vector3 value, out float x, out float y, out float z)
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
}