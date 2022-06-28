using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace StatUtil {

    [CreateAssetMenu()]
    public class Stat : RestorableScriptableObject {
        public float baseValue;
        public double calculatedValue;

        public List<StatFlat> statFlats;
        public List<StatIncrease> statIncreases;
        public List<StatMore> statMores;

        private List<StatFormula> _statFormulas;
        
        private void OnEnable() {

        }

        private void OnDisable() {

        }

        protected override void CleanUpPlayState() {
            UpdateValue();
        }

        private void OnValidate() {
            UpdateValue();
        }

        public void RegisterModifier(StatModifier statModifier) {
            switch (statModifier) {
                case StatFlat sf:
                    if (!statFlats.Contains(sf)) statFlats.Add(sf); break;
                case StatIncrease si:
                    if (!statIncreases.Contains(si)) statIncreases.Add(si); break;
                case StatMore sm:
                    if (!statMores.Contains(sm)) statMores.Add(sm); break;
                case StatFormula sfo:
                    if (!_statFormulas.Contains(sfo)) _statFormulas.Add(sfo); break;
            }
            UpdateValue();
        }

        public void RemoveModifier(StatModifier statModifier) {
            switch (statModifier) {
                case StatFlat sf:
                    if (statFlats.Contains(sf)) statFlats.Remove(sf); break;
                case StatIncrease si:
                    if (statIncreases.Contains(si)) statIncreases.Remove(si); break;
                case StatMore sm:
                    if (statMores.Contains(sm)) statMores.Remove(sm); break;
                case StatFormula sfo:
                    if (_statFormulas.Contains(sfo)) _statFormulas.Remove(sfo); break;
            }
            UpdateValue();
        }

        public void UpdateValue() {
            double temp = baseValue;
            foreach (var sf in statFlats) {
                if (sf.statSourceOverwrite) {
                    //Propagate down (also means you can't depend on a stat value down the chain
                    sf.statSourceOverwrite.UpdateValue();
                    temp += sf.statSourceOverwrite.calculatedValue;
                }
                else {
                    temp += sf.value;
                }
            }

            double siAccumulator = 0;
            foreach (var si in statIncreases) {
                siAccumulator += si.value;
            }
            temp *= 1 + siAccumulator;
            foreach (var sm in statMores) {
                temp *= 1 + sm.value;
            }
            calculatedValue = temp;
        }
    }

    [Serializable]
    public abstract class StatModifier {
        public float value;
    }

    [Serializable]
    public class StatFormula : StatModifier{
        
    }

    [Serializable]
    public class StatFlat : StatModifier {
        public Stat statSourceOverwrite;
    }

    [Serializable]
    public class StatIncrease : StatModifier {

    }

    [Serializable]
    public class StatMore : StatModifier {

    }
}

