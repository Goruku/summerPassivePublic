using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponCurve : ScriptableObject {
    public float amplitude = 1;
    public AnimationCurve curve;
    public CurveOption curveOption = CurveOption.Smooth;
    [Range(0.001f,1)]
    public float step = 0.001f;
    [Range(0,1)]
    public float threshold = 1;
    public bool invertThreshold;


    public WeaponCurvePacket Evaluate(float t) {
        var curveValue = amplitude * curveOption switch
        { 
            CurveOption.FloorStep => Mathf.Floor(curve.Evaluate(t) / step)*step,
            CurveOption.CeilStep => Mathf.Ceil(curve.Evaluate(t) / step)*step,
            CurveOption.Smooth => curve.Evaluate(t),
            _ => curve.Evaluate(t)
        };
        
        return new WeaponCurvePacket(curveValue, invertThreshold ? curveValue <= threshold : curveValue >= threshold);
    }

    [Serializable]
    public struct WeaponCurvePacket {
        public float value;
        public bool thresholdReach;

        public WeaponCurvePacket(float value, bool thresholdReach) {
            this.value = value;
            this.thresholdReach = thresholdReach;
        }
    }

    public enum CurveOption {
        FloorStep, CeilStep, Smooth
    }
}
