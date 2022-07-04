﻿using System;
using System.Collections.Generic;
using StatUtil;
using UnityEngine;


namespace Passive {
    [RequireComponent(typeof(PassiveNode))]
    [ExecuteAlways]
    public class PassiveStat : MonoBehaviour {
        public Stat stat;
        public List<StatFlat> statFlats;
        public List<StatIncrease> statIncreases;
        public List<StatMore> statMores;

        private PassiveNode _passiveNode;

        private void Awake() {
            _passiveNode = gameObject.GetComponent<PassiveNode>();
            _passiveNode.AddStat(this);
        }

        private void OnDestroy() {
            if(!_passiveNode) return;
            _passiveNode.RemoveStat(this);
        }

        public void RegisterAllModifiers() {
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

        public void RemoveAllModifiers() {
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