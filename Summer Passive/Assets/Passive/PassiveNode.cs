using System;
using System.Collections.Generic;
using StatUtil;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util;

namespace Passive {
    [ExecuteAlways]
    public class PassiveNode : MonoBehaviour {
        public delegate void NodeAction(bool allocated);

        public List<PassiveLink> links;

        public string passiveName = "";
        public NodeAction NodeActions = b => { };

        private RectTransform _rectTransform;

        [SerializeField, GetSet("allocated")]
        private bool _allocated;

        public bool allocated {
            get { return _allocated;}
            set {
                _allocated = value;
                NodeActions(_allocated);
                UpdateLinks();
            }
        }
        public int neighbourNeeded = 1;
        public int id;

        // Start is called before the first frame update
        private void Start() { }

        // Update is called once per frame
        private void Update() { }

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Press() {
            if (allocated && !CheckIfSafeRemove()) return;
            if (!allocated && !CheckAvailability()) return;
            allocated = !allocated;
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
    }

    public enum NodeState {
        UnAllocated,
        Allocated
    }
}
