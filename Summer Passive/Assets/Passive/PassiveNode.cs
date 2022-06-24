using System;
using System.Collections.Generic;
using StatUtil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Passive {
    [ExecuteAlways]
    public class PassiveNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler {
        public List<PassiveLink> links;
        public GameObject tooltipPrefab;

        public Stat stat;
        public List<StatFlat> statFlats;
        public List<StatIncrease> statIncreases;
        public List<StatMore> statMores;

        private RectTransform _rectTransform;
        private Button _button;

        public bool allocated;
        public bool root;
        public int id;
        public string nameText;
        public string descriptionText;

        private PassiveTooltip _passiveTooltip;
        public bool hovered;
        public int tooltipFixingTime;
        public int hoverDuration;

        // Start is called before the first frame update
        private void Start() {

        }

        // Update is called once per frame
        private void Update() {
            
        }

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
        }

        public void OnPointerMove(PointerEventData eventData) {
           //_passiveTooltip._rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            hovered = true;
            _passiveTooltip = Instantiate(tooltipPrefab, transform).GetComponent<PassiveTooltip>();
            _passiveTooltip._rectTransform.anchoredPosition +=
                new Vector2(_passiveTooltip._rectTransform.sizeDelta.x*0.5f + _rectTransform.sizeDelta.x*0.5f,0);
            _passiveTooltip.header.text = nameText;
            _passiveTooltip.description.text = descriptionText;
        }

        public void OnPointerExit(PointerEventData eventData) {
            hovered = false;
            Destroy(_passiveTooltip.gameObject);
        }

        public void Press() {
            if (allocated && !CheckIfSafeRemove()) return;
            if (!allocated && !CheckAvailability()) return;
            allocated = !allocated;
            UpdateGraphics();
            if (allocated) {
                RegisterAllModifiers();
            }
            else {
                 RemoveAllModifiers();
            }
        }

        private void RegisterAllModifiers() {
            if (!stat) return;
            foreach (var sf in statFlats) {
                stat.RegisterModifier(sf);
            }

            foreach (var si in statIncreases) {
                stat.RegisterModifier(si);
            }

            foreach (var sm in statMores) {
                stat.RegisterModifier(sm);
            }
        }

        private void RemoveAllModifiers() {
            if (!stat) return;
            foreach (var sf in statFlats) {
                stat.RemoveModifier(sf);
            }

            foreach (var si in statIncreases) {
                stat.RemoveModifier(si);
            }

            foreach (var sm in statMores) {
                stat.RemoveModifier(sm);
            }
        }

        public void UpdateGraphics() {
            UpdateState();
            UpdateLinks();
        }

        public void UpdateState() {
            var nodeState = ComputeState();
            var colors = _button.colors;
            colors.normalColor = nodeState.Color();
            colors.selectedColor = nodeState.Color();
            _button.colors = colors;
        }

        private NodeState ComputeState() {
            return allocated switch {
                true => NodeState.Allocated,
                false => NodeState.UnAllocated
            };
        }

        private void UpdateLinks() {
            foreach (PassiveLink link in links) {
                link.UpdateState();
            }
        }
        
        private bool CheckAvailability() {
            if (root) return true;
            bool available = false;
            foreach (var link in links) {
                if (link.IsDependant(this) && !link.GetLinkedPoint(this).allocated) return false;
                if (!available)
                    available |= link.AllowsTravel();
            }
            return available;
        }
        private bool CheckIfSafeRemove() {
            Dictionary<int, bool> searchedPoints = new () {{ GetInstanceID(), false}};
            foreach (PassiveLink link in links) {
                PassiveNode childNode = link.GetLinkedPoint(this);
                if (!childNode.allocated) continue;
                if (link.IsMandatory(this) || !ReachesRoot(childNode, searchedPoints)) return false;
            }
            return true;
        }

        private static bool ReachesRoot(PassiveNode passiveNode, Dictionary<int, bool> searchedPoints) {
            if (searchedPoints.TryGetValue(passiveNode.GetInstanceID(), out var canReach)) return canReach;
            if (passiveNode.root) return true;
            
            searchedPoints.Add(passiveNode.GetInstanceID(), false);
            foreach (PassiveLink link in passiveNode.links) {
                PassiveNode childNode = link.GetLinkedPoint(passiveNode);
                if (!childNode.allocated || !ReachesRoot(childNode, searchedPoints)) continue;
                searchedPoints[passiveNode.GetInstanceID()] = true;
                break;
            }
            return searchedPoints[passiveNode.GetInstanceID()];
        }

        public RectTransform GetRectTransform() {
            return _rectTransform;
        }
    }

    public enum NodeState {
        UnAllocated,
        Allocated
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
