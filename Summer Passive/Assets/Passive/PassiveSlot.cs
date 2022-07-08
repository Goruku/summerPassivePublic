using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Passive;
using UnityEngine;

namespace Passive {
    [RequireComponent(typeof(PassiveNode))]
    [ExecuteAlways]
    public class PassiveSlot : MonoBehaviour {
        public PassiveTree passiveTree;

        private void Awake() {
            foreach(var node in passiveTree._passiveNodes) {

            }
        }

        private void OnDisable() {
            throw new NotImplementedException();
        }

        public void Update() {

        }
    }
}

