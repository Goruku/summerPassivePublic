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
        public LinkDirection mandatoryDirection;

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        public void UpdateState() {
            linkState = ComputeState();
            GetComponent<RawImage>().color = linkState.Color();
        }

        public void UpdateDimension() {
            var linkShape = GetComponent<RectTransform>();
            var leftPosition = left.GetComponent<RectTransform>().anchoredPosition3D;
            var rightPosition = right.GetComponent<RectTransform>().anchoredPosition3D;
            linkShape.position = (rightPosition - leftPosition) * 0.5f;
            linkShape.rotation = Quaternion.Euler(0, 0, MathF.Atan2(rightPosition.y - leftPosition.y, rightPosition.x - leftPosition.x) * Mathf.Rad2Deg);
            linkShape.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10);
            linkShape.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Vector3.Distance(rightPosition, leftPosition));
            linkShape.anchoredPosition3D = leftPosition + (rightPosition - leftPosition) * 0.5f;
        }

        private LinkState ComputeState() {
            return mandatoryDirection switch {
                LinkDirection.None when left.allocated != right.allocated => LinkState.Available,
                LinkDirection.Left when !left.allocated => LinkState.Mandated,
                LinkDirection.Right when !right.allocated => LinkState.Mandated,
                LinkDirection.Left when left.allocated => LinkState.Mandates,
                LinkDirection.Right when right.allocated => LinkState.Mandates,
                _ when left.allocated && right.allocated => LinkState.Taken,
                _ => LinkState.Unavailable
            };
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
            return GetSide(point) == mandatoryDirection;
        }

        public bool IsDependant(PassivePoint point) {
            return GetSide(point).Flip() == mandatoryDirection;
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

