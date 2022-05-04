using UnityEngine;

namespace Player.Ability {
    public class Dash : CooldownAbility {

        public float dashVelocity;

        private Vector3 direction = Vector3.zero;

        public Dash(float dashVelocity) : base(2f) {
            this.dashVelocity = dashVelocity;
        }

        public void shiftDirection(Vector3 shiftDirection) {
            direction = Vector3.Normalize(direction + shiftDirection);
        }

        public override void Fire() {
            transform.position += direction * dashVelocity;
        }

    }
}