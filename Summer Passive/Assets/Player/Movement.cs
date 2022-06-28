using KeyManagement;
using Player.Ability;
using UnityEngine;
using StatUtil;

namespace Player {
    public class Movement : MonoBehaviour {

        public KeyManager keyManager;

        public Dash dash;

        public Stat moveSpeed;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            foreach (KeyInfo keyInfo in keyManager.movementKeys) {
                if (keyInfo.doubleTap || (keyManager.movementCombo.held && keyInfo.held)) {
                    dash.shiftDirection(keyInfo.direction);
                    dash.Arm();
                }
            }
        }

        private void FixedUpdate() {
            //General movement
            foreach (var keyInfo in keyManager.movementKeys) {
                Move(keyInfo.held, keyInfo.direction, (float) moveSpeed.calculatedValue);
            }
            dash.Trigger();
        }

        private void Move(bool condition, Vector3 direction, float magnitude) {
            if (condition)
                transform.position += direction * magnitude;
        }

    }
}
