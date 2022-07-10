using System;
using System.Collections.Generic;
using StatUtil;
using UnityEngine;


namespace Passive {
    [RequireComponent(typeof(PNode))]
    [ExecuteAlways]
    public class PStat : MonoBehaviour {
        public Stat stat;
        public List<StatFlat> statFlats;
        public List<StatIncrease> statIncreases;
        public List<StatMore> statMores;

        private PNode _pNode;

        private void Awake() {
            _pNode = gameObject.GetComponent<PNode>();
            _pNode.NodeActions += ChangeAllModifiers;
        }

        private void OnDisable() {
            if(!_pNode) return;
            _pNode.NodeActions -= ChangeAllModifiers;
        }

        private void ChangeAllModifiers(bool allocated) {
            if (allocated)
                RegisterAllModifiers();
            else
                RemoveAllModifiers();
        }

        private void RegisterAllModifiers() {
            if (!stat) return;
            foreach (var sf in statFlats) {
                stat.RegisterModifier(sf);
            }

            foreach (var si in statIncreases) {
                stat.RegisterModifier(si);
            }

            foreach (var sm in statMores) {
                stat.RegisterModifier(sm);
            }
        }

        private void RemoveAllModifiers() {
            if (!stat) return;
            foreach (var sf in statFlats) {
                stat.RemoveModifier(sf);
            }

            foreach (var si in statIncreases) {
                stat.RemoveModifier(si);
            }

            foreach (var sm in statMores) {
                stat.RemoveModifier(sm);
            }
        }
    }
}