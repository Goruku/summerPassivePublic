using UnityEngine;

namespace Player {
    public class ControlUtility {
        public static Vector3 GetDirection(KeyCode keyCode) {
            return keyCode switch {
                KeyCode.W => Vector3.up,
                KeyCode.D => Vector3.right,
                KeyCode.S => Vector3.down,
                KeyCode.A => Vector3.left,
                _ => Vector3.zero
            };
        }
        
        public class KeyInfo {
            public float Timestamp;

            public KeyInfo(float time) {
                this.Timestamp = time;
            }

            public void Update() {
                Timestamp = Time.time;
            }
        }
    }
}