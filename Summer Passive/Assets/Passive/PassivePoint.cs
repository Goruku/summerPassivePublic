using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Passive {
    public class PassivePoint : MonoBehaviour {
        public PassiveTree passiveTree;
        private Button _associatedButton;
        public List<PassivePoint> mandatory;
        public List<PassivePoint> parents;
        public List<PassivePoint> children;
        public bool allocated;
        public bool root;

        private bool propagated;

        // Start is called before the first frame update
        void Start() {
            _associatedButton = GetComponent<Button>();
            _associatedButton.onClick.AddListener(Toggle);

            if (root) {
                Propagate();
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
            if (!CheckDependency()) return false;
            allocated = false;
            var colors = _associatedButton.colors;
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            _associatedButton.colors = colors;
            return allocated;
        }

        private void Propagate() {
            if (!propagated)
                foreach (PassivePoint child in children) {
                    child.parents.Add(this);
                    child.Propagate();
                }
            propagated = true;
        }

        public bool CheckAvailability() {
            bool fulfilled = false;
            foreach (var linkedPoint in parents) {
                fulfilled |= linkedPoint.allocated;
            }
            foreach (var mandatoryPoint in mandatory) {
                fulfilled &= mandatoryPoint.allocated;
            }
            return fulfilled;
        }

        public bool CheckDependency() {
            if (root) return false;
            foreach (PassivePoint child in children) {
                if (child.allocated) {
                    foreach (PassivePoint mandatory in child.mandatory) {
                        if (mandatory.Equals(this)) return false;
                    }
                    bool hasValidParent = false;
                    foreach (PassivePoint parent in child.parents) {
                        if (!parent.Equals(this) && parent.allocated)
                            hasValidParent = true;
                    }
                    if (!hasValidParent) return false;
                }
            }
            return true;
        }
    }
}
