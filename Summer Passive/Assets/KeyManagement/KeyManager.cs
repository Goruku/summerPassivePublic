using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyManagement {
    public class KeyManager : MonoBehaviour {
        private readonly Dictionary<KeyCode, KeyInfo> _keyInfo;

        public List<KeyInfo> movementKeys;

        public KeyInfo movementCombo;

        public void Start() {
            foreach (var movementKey in movementKeys) {
                _keyInfo.Add(movementKey.keyCode, movementKey);
            }
            _keyInfo.Add(movementCombo.keyCode, movementCombo);
        }
        
        public KeyInfo Get(KeyCode key) {
            return _keyInfo[key];
        }
    }
}