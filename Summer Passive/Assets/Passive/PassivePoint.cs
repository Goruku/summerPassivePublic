using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Passive {
    public class PassivePoint : MonoBehaviour {
        public PassiveTree passiveTree;
        public List<PassiveLink> links;

        public bool allocated;
        public bool root;

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
            var passiveButton = GetComponent<Button>();
            var colors = passiveButton.colors;
            colors.normalColor = Color.yellow;
            colors.selectedColor = Color.yellow;
            passiveButton.colors = colors;
            UpdateLinks();
            return allocated;
        }

        public bool Unallocate() {
            if (!CheckIfSafeRemove()) return false;
            allocated = false;
            var passiveButton = GetComponent<Button>();
            var colors = passiveButton.colors;
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            passiveButton.colors = colors;
            UpdateLinks();
            return allocated;
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
                var linkAllocated = link.GetLinkedPoint(this).allocated;
                if (link.IsDependant(this) && !linkAllocated) return false;
                available |= linkAllocated;
            }
            return available;
        }
        private bool CheckIfSafeRemove() {
            Dictionary<int, bool> searchedPoints = new () {{ GetInstanceID(), false}};
            foreach (PassiveLink link in links) {
                PassivePoint childPoint = link.GetLinkedPoint(this);
                if (!childPoint.allocated) continue;
                if (link.IsMandatory(this) || !ReachesRoot(childPoint, searchedPoints)) return false;
            }
            return true;
        }

        private static bool ReachesRoot(PassivePoint passivePoint, Dictionary<int, bool> searchedPoints) {
            if (searchedPoints.TryGetValue(passivePoint.GetInstanceID(), out var canReach)) return canReach;
            if (passivePoint.root) return true;
            
            searchedPoints.Add(passivePoint.GetInstanceID(), false);
            foreach (PassiveLink link in passivePoint.links) {
                PassivePoint childPoint = link.GetLinkedPoint(passivePoint);
                if (!childPoint.allocated || !ReachesRoot(childPoint, searchedPoints)) continue;
                searchedPoints[passivePoint.GetInstanceID()] = true;
                break;
            }
            return searchedPoints[passivePoint.GetInstanceID()];
        }
    }
}
