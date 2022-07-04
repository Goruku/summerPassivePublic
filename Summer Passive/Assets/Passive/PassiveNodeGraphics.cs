using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Passive {
    [RequireComponent(typeof(PassiveNode))]
    [ExecuteAlways]
    public class PassiveNodeGraphics : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler{
        
        public GameObject tooltipPrefab;
        private PassiveTooltip _passiveTooltip;

        public string nameText;
        public string descriptionText;
        
        public TextMeshProUGUI textGUI;

        public bool hovered;
        public int tooltipFixingTime;
        public int hoverDuration;

        private RectTransform _rectTransform;
        private PassiveNode _passiveNode;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _passiveNode = GetComponent<PassiveNode>();
            _passiveNode.SetGraphics(this);
        }

        private void OnDestroy() {
            _passiveNode.RemoveGraphics(this);
        }


        public void OnPointerMove(PointerEventData eventData) {
            //_passiveTooltip._rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!Application.isPlaying) return;
            hovered = true;
            _passiveTooltip = Instantiate(tooltipPrefab, transform).GetComponent<PassiveTooltip>();
            _passiveTooltip.rectTransform.anchoredPosition +=
                new Vector2(_passiveTooltip.rectTransform.sizeDelta.x * 0.5f + _rectTransform.sizeDelta.x * 0.5f, 0);
            _passiveTooltip.header.text = nameText;
            _passiveTooltip.description.text = descriptionText;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!Application.isPlaying) return;
            hovered = false;
            Destroy(_passiveTooltip.gameObject);
        }
        
        public void SetText(String text) {
            textGUI.text = text;
        }
    }
}