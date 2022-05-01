using UnityEngine;

namespace Player.Ability {
    public class CooldownAbility : MonoBehaviour {
        
        public bool armed;
        public float lastActivation;
        public float cooldown;

        public CooldownAbility(float cooldown) {
            this.cooldown = cooldown;
        }
        
        public bool Arm() {
            float currentTime = Time.time;
            if (!(currentTime - lastActivation >= cooldown) || !ArmCondition()) return false;
            armed = true;
            return true;
        }

        public bool Trigger() {
            if (!armed || !TriggerCondition()) return false;
            Fire();
            armed = false;
            lastActivation = Time.time;
            return true;
        }

        public virtual void Fire() {
            
        }

        public virtual bool ArmCondition() {
            return true;
        }
        
        public virtual bool TriggerCondition() {
            return true;
        }
    }
}