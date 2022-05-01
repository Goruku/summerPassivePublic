using UnityEngine;

namespace Player.Ability {
    public class Dash : CooldownAbility {

        public float dashVelocity;

        public Vector3 direction = Vector3.zero;

        public Dash(float dashVelocity) : base(2f) {
            
        }

        public override void Fire() {
            transform.position += direction * dashVelocity;
        }

    }
}