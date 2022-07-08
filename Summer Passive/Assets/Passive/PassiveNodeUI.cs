using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Passive {
    [RequireComponent(typeof(PassiveNode))]
    [RequireComponent(typeof(Button))]
    [ExecuteAlways]
    public class PassiveNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler{
        
        public GameObject tooltipPrefab;
        private PassiveTooltip _passiveTooltip;

        public string overloadName = "";

        public string nameText;
        public string descriptionText;
        
        public TextMeshProUGUI textGUI;

        public bool hovered;
        public int tooltipFixingTime;
        public int hoverDuration;

        private RectTransform _rectTransform;
        private PassiveNode _passiveNode;
        private Button _button;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _passiveNode = GetComponent<PassiveNode>();
            _button = GetComponent<Button>();
            _passiveNode.NodeActions += UpdateGraphics;
        }

        private void OnDisable() {
            _passiveNode.NodeActions -= UpdateGraphics;
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

        public void UpdateGraphics(bool allocated) {
            var nodeState = ComputeState(allocated);
            var colors = _button.colors;
            colors.normalColor = nodeState.Color();
            colors.selectedColor = nodeState.Color();
            _button.colors = colors;
            textGUI.text = overloadName == "" ?  _passiveNode.passiveName : overloadName;
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