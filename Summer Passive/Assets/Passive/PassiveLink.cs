using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Passive {
    public class PassiveLink : MonoBehaviour {

        public PassivePoint left;
        public PassivePoint right;
        public LinkState linkState;
        public LinkDirection mandatory;

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        public void UpdateState() {
            linkState = ComputeState();
            GetComponent<RawImage>().color = linkState.Color();
        }

        private LinkState ComputeState() {
            if (!left.allocated && !right.allocated) return LinkState.Unavailable;
            if (left.allocated && right.allocated) return LinkState.Taken;
            if (mandatory == LinkDirection.None) return LinkState.Available;
            if ((mandatory == LinkDirection.Left && left.allocated) ||
                (mandatory == LinkDirection.Right && right.allocated)) return LinkState.Mandates;
            return LinkState.Mandated;
        }

        public LinkDirection GetSide(PassivePoint point) {
            if (point.GetInstanceID() == left.GetInstanceID()) return LinkDirection.Left;
            return LinkDirection.Right;
        }

        public PassivePoint GetLinkedPoint(PassivePoint point) {
            if (point.GetInstanceID() == left.GetInstanceID()) return right;
            return left;
        }

        public bool IsMandatory(PassivePoint point) {
            return GetSide(point) == mandatory;
        }

        public bool IsDependant(PassivePoint point) {
            return GetSide(point).Flip() == mandatory;
        }

    }

    public enum LinkDirection {
        None,
        Left,
        Right,
    }

    public enum LinkState {
        Unavailable,
        Available,
        Taken,
        Mandates,
        Mandated,
    }

    static class LinkEnumMethods {
        public static LinkDirection Flip(this LinkDirection linkDirection) {
            return linkDirection switch {
                LinkDirection.Left => LinkDirection.Right,
                LinkDirection.Right => LinkDirection.Left,
                _ => LinkDirection.None
            };
        }

        public static Color Color(this LinkState linkState) {
            return linkState switch {
                LinkState.Unavailable => UnityEngine.Color.white,
                LinkState.Taken => UnityEngine.Color.yellow,
                LinkState.Available => UnityEngine.Color.cyan,
                LinkState.Mandates => UnityEngine.Color.green,
                LinkState.Mandated => UnityEngine.Color.red,
                _ => UnityEngine.Color.black
            };
        }
    }
}

