using System;
using System.Collections.Generic;
using StatUtil;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Passive {
    [ExecuteAlways]
    public class PassiveNode : MonoBehaviour {
        public List<PassiveLink> links;

        private List<PassiveStat> _passiveStats = new ();
        private PassiveNodeGraphics _passiveNodeGraphics;
        private bool _hasCustomGraphics = false;

        private RectTransform _rectTransform;
        private Button _button;

        public bool allocated;
        public int neighbourNeeded = 1;
        public int id;

        // Start is called before the first frame update
        private void Start() { }

        // Update is called once per frame
        private void Update() { }

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _button = GetComponent<Button>();
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
            foreach (var stat in _passiveStats) {
                stat.RegisterAllModifiers();
            }
        }

        private void RemoveAllModifiers() {
            foreach (var stat in _passiveStats) {
                stat.RemoveAllModifiers();
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
            if (neighbourNeeded == 0) return true;
            if (GetNeighbourCount() < neighbourNeeded) return false;
            bool available = false;
            foreach (var link in links) {
                if (link.IsDependant(this) && !link.GetLinkedPoint(this).allocated) return false;
                if (!available)
                    available |= link.AllowsTravel();
            }

            return available;
        }

        private bool CheckIfSafeRemove() {
            Dictionary<int, bool> searchedPoints = new() { { GetInstanceID(), false } };
            foreach (var link in links) {
                var childNode = link.GetLinkedPoint(this);
                if (!childNode.allocated) continue;
                if (childNode.GetNeighbourCount() - 1 < childNode.neighbourNeeded) return false;
                if (link.IsMandatory(this) || !ReachesRoot(childNode, searchedPoints)) return false;
            }

            return true;
        }

        public int GetNeighbourCount() {
            var neighbourCount = 0;
            foreach (var link in links) {
                if (!link.GetLinkedPoint(this).allocated) continue;
                neighbourCount++;
            }

            return neighbourCount;
        }

        private static bool ReachesRoot(PassiveNode passiveNode, Dictionary<int, bool> searchedPoints) {
            if (searchedPoints.TryGetValue(passiveNode.GetInstanceID(), out var canReach)) return canReach;
            if (passiveNode.neighbourNeeded == 0) return true;

            searchedPoints.Add(passiveNode.GetInstanceID(), false);
            foreach (PassiveLink link in passiveNode.links) {
                if (!link.travels) continue;
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

        public void AddStat(PassiveStat passiveStat) {
            _passiveStats.Add(passiveStat);
        }

        public void RemoveStat(PassiveStat passiveStat) {
            _passiveStats.Remove(passiveStat);
        }

        public void SetGraphics(PassiveNodeGraphics passiveNodeGraphics) {
            _passiveNodeGraphics = passiveNodeGraphics;
            _hasCustomGraphics = true;
        }

        public void RemoveGraphics(PassiveNodeGraphics passiveNodeGraphics) {
            //TODO: modular?
            _passiveNodeGraphics = null;
            _hasCustomGraphics = false;
        }

        public ref PassiveNodeGraphics GetGraphics() {
            return ref _passiveNodeGraphics;
        }

        public bool HasCustomGraphics() {
            return _hasCustomGraphics;
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
