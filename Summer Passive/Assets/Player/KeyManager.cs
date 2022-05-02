using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player {
    public class KeyManager : MonoBehaviour {
        private readonly Dictionary<KeyCode, KeyInfo> _keyInfo;

        public List<KeyInfo> movementKeys;

        public void Start() {
            foreach (var movementKey in movementKeys) {
                _keyInfo.Add(movementKey.keyCode, movementKey);
            }
        }

        public KeyInfo Get(KeyCode key) {
            return _keyInfo[key];
        }
    }
}