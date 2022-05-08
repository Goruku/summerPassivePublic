using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Passive {
    public class PassivePoint : MonoBehaviour {
        public PassiveTree passiveTree;
        private Button _associatedButton;
        public List<PassivePoint> mandatoryTo;
        public List<PassivePoint> linked;

        public List<PassivePoint> mandatory;
        
        public bool allocated;
        public bool root;

        // Start is called before the first frame update
        void Start() {
            _associatedButton = GetComponent<Button>();
            _associatedButton.onClick.AddListener(Toggle);
            foreach (PassivePoint childPoints in mandatoryTo) {
                childPoints.mandatory.Add(this);
            }
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
            var colors = _associatedButton.colors;
            colors.normalColor = Color.yellow;
            colors.selectedColor = Color.yellow;
            _associatedButton.colors = colors;
            return allocated;
        }

        public bool Unallocate() {
            if (!CheckIfSafeRemove()) return false;
            allocated = false;
            var colors = _associatedButton.colors;
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            _associatedButton.colors = colors;
            return allocated;
        }
        
        public bool CheckAvailability() {
            bool available = false;
            foreach (var linkedPoint in linked) {
                available |= linkedPoint.allocated;
            }
            foreach (var mandatoryPoint in mandatory) {
                available &= mandatoryPoint.allocated;
            }
            return available;
        }
        private bool CheckIfSafeRemove() {
            foreach (PassivePoint mandatedPoint in mandatoryTo) {
                if (mandatedPoint.allocated) return false;
            }
            Dictionary<int, bool> searchedPoints = new () {{ GetInstanceID(), false}};
            foreach (PassivePoint childPoint in linked) {
                if (childPoint.allocated && !ReachesRoot(childPoint, searchedPoints)) return false;
            }
            return true;
        }

        private bool ReachesRoot(PassivePoint passivePoint, Dictionary<int, bool> searchedPoints) {
            if (passivePoint.root) return true;
            if (searchedPoints.TryGetValue(passivePoint.GetInstanceID(), out var canReach)) return canReach;
            
            searchedPoints.Add(passivePoint.GetInstanceID(), false);
            foreach (PassivePoint childPoint in passivePoint.linked) {
                if (!childPoint.allocated || !ReachesRoot(childPoint, searchedPoints)) continue;
                searchedPoints[passivePoint.GetInstanceID()] = true;
                break;
            }
            return searchedPoints[passivePoint.GetInstanceID()];
        }
    }
}
