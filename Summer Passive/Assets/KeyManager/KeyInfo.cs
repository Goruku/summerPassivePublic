using UnityEditor.UI;
using UnityEngine;

namespace Player {
    public class KeyInfo : MonoBehaviour {
        public KeyCode keyCode;
        public bool held;
        public bool down;
        public bool up;
        public bool doubleTap;
        public float doubleTapWindow = 0.5f;
        public float timestamp;
        public float lastTimestamp;
        public Vector3 direction;

        public KeyInfo(KeyCode keyCode, float timestamp, Vector3 direction) {
            this.keyCode = keyCode;
            this.timestamp = timestamp;
            this.direction = direction;
        }

        public void Update() {
            if (Input.GetKeyDown(keyCode)) {
                lastTimestamp = timestamp;
                timestamp = Time.time;
                down = true;
            } else {
                down = false;
            }
            doubleTap = Time.time - lastTimestamp < doubleTapWindow && down;
            held = Input.GetKey(keyCode);
            up = Input.GetKeyUp(keyCode);
        }

        public void setDirection(Vector3 direction) {
            this.direction = direction;
        }
    }
}