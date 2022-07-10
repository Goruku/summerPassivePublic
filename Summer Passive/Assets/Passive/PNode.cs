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
    public class PNode : MonoBehaviour {

        public List<PLink> links;

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

        public NodeAction NeighbourVigil = b => { };

        [SerializeField, GetSet("neighbourNeeded")]
        private int _neighbourNeeded = 1;
        public int neighbourNeeded {
            get { return _neighbourNeeded;}
            set {
                _neighbourNeeded = value;
                NeighbourVigil(allocated);
            }
        }
        
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
            foreach (PLink link in links) {
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

        private static bool ReachesRoot(PNode pNode, Dictionary<int, bool> searchedPoints) {
            if (searchedPoints.TryGetValue(pNode.GetInstanceID(), out var canReach)) return canReach;
            if (pNode.neighbourNeeded <= 0) return true;

            searchedPoints.Add(pNode.GetInstanceID(), false);
            foreach (PLink link in pNode.links) {
                if (!link.travels) continue;
                PNode childNode = link.GetLinkedPoint(pNode);
                if (!childNode.allocated || !ReachesRoot(childNode, searchedPoints)) continue;
                searchedPoints[pNode.GetInstanceID()] = true;
                break;
            }

            return searchedPoints[pNode.GetInstanceID()];
        }

        public RectTransform GetRectTransform() {
            return _rectTransform;
        }
        
        public delegate void NodeAction(bool allocated);
    }

    public enum NodeState {
        UnAllocated,
        Allocated
    }
}
