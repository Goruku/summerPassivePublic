using System;
using System.Collections.Generic;
using UnityEngine;

namespace KeyManagement {
    public class KeyManager : MonoBehaviour {
        public List<KeyInfo> managedKeys;

        public List<KeyInfo> movementKeys;
        public KeyInfo movementCombo;
        
        public void Update() {
            foreach (var key in managedKeys) {
                key.UpdateKeyInfo();
            }
        }

    }
}