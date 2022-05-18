using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Passive {
    public class PassiveNode : MonoBehaviour {
        public PassiveTree passiveTree;
        public List<PassiveLink> links;

        public bool allocated;
        public bool root;
        public int id;
        public string text;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Toggle() {
            if (allocated) {
                Unallocate();
            }
            else {
                Allocate();
            }
        }

        public bool Allocate() {
            if (!CheckAvailability()) return false;
            allocated = true;
            UpdateState();
            UpdateLinks();
            return allocated;
        }

        public bool Unallocate() {
            if (!CheckIfSafeRemove()) return false;
            allocated = false;
            UpdateState();
            UpdateLinks();
            return allocated;
        }

        public void UpdateState() {
            var nodeState = ComputeState();
            var passiveButton = GetComponent<Button>();
            var colors = passiveButton.colors;
            colors.normalColor = nodeState.Color();
            colors.selectedColor = nodeState.Color();
            passiveButton.colors = colors;
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
        
        public bool CheckAvailability() {
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
