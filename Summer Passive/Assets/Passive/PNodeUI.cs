using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Passive {
    [RequireComponent(typeof(PNode))]
    [RequireComponent(typeof(Button))]
    [ExecuteAlways]
    public class PNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler{
        
        public GameObject tooltipPrefab;
        private PTooltip _pTooltip;

        public string overloadName = "";

        public string nameText;
        public string descriptionText;
        
        public TextMeshProUGUI textGUI;

        public bool hovered;
        public int tooltipFixingTime;
        public int hoverDuration;

        private RectTransform _rectTransform;
        private PNode _pNode;
        private Button _button;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _pNode = GetComponent<PNode>();
            _button = GetComponent<Button>();
            _pNode.NodeActions += UpdateGraphics;
        }

        private void OnDisable() {
            _pNode.NodeActions -= UpdateGraphics;
        }


        public void OnPointerMove(PointerEventData eventData) {
            //_passiveTooltip._rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (!Application.isPlaying) return;
            hovered = true;
            _pTooltip = Instantiate(tooltipPrefab, transform).GetComponent<PTooltip>();
            _pTooltip.rectTransform.anchoredPosition +=
                new Vector2(_pTooltip.rectTransform.sizeDelta.x * 0.5f + _rectTransform.sizeDelta.x * 0.5f, 0);
            _pTooltip.header.text = nameText;
            _pTooltip.description.text = descriptionText;
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (!Application.isPlaying) return;
            hovered = false;
            Destroy(_pTooltip.gameObject);
        }

        public void UpdateGraphics(bool allocated) {
            var nodeState = ComputeState(allocated);
            var colors = _button.colors;
            colors.normalColor = nodeState.Color();
            colors.selectedColor = nodeState.Color();
            _button.colors = colors;
            textGUI.text = overloadName == "" ?  _pNode.passiveName : overloadName;
        }

        private NodeState ComputeState(bool allocated) {
            return allocated switch {
                true => NodeState.Allocated,
                false => NodeState.UnAllocated
            };
        }
    }
    
    static class NodeEnumMethods {
        public static Color Color(this NodeState nodeState) {
            return nodeState switch {
                NodeState.UnAllocated => UnityEngine.Color.white,
                NodeState.Allocated => UnityEngine.Color.yellow,
                _ => UnityEngine.Color.black
            };
        }
    }
}