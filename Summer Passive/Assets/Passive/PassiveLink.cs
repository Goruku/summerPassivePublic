using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Passive {
    public class PassiveLink : MonoBehaviour {

        public PassiveNode left;
        public PassiveNode right;
        public LinkState linkState;
        public LinkDirection direction;
        public bool mandatory;

        private RawImage _rawImage;
        private RectTransform _rectTransform;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() { }

        private void Awake() {
            _rawImage = GetComponent<RawImage>();
            _rectTransform = GetComponent<RectTransform>();
            LinkComponents();
        }

        public void LinkComponents() {
            _rawImage = GetComponent<RawImage>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateState() {
            linkState = ComputeState();
            _rawImage.color = linkState.Color();
        }

        public void UpdateDimension() {
            var leftPosition = left.GetRectTransform().anchoredPosition3D;
            var rightPosition = right.GetRectTransform().anchoredPosition3D;
            _rectTransform.position = (rightPosition - leftPosition) * 0.5f;
            _rectTransform.rotation = Quaternion.Euler(0, 0, MathF.Atan2(rightPosition.y - leftPosition.y, rightPosition.x - leftPosition.x) * Mathf.Rad2Deg);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Vector3.Distance(rightPosition, leftPosition));
            _rectTransform.anchoredPosition3D = leftPosition + (rightPosition - leftPosition) * 0.5f;
        }
        
        private LinkState ComputeState() {
            if (left.allocated && right.allocated) return LinkState.Taken;
            if (AllowsTravel()) {
                return mandatory ? LinkState.Mandates : LinkState.Available;
            }
            return mandatory ?  LinkState.Mandated : LinkState.Unavailable;
        }

        public LinkDirection GetSide(PassiveNode node) {
            if (node.GetInstanceID() == left.GetInstanceID()) return LinkDirection.Left;
            return LinkDirection.Right;
        }

        public PassiveNode GetLinkedPoint(PassiveNode node) {
            if (node.GetInstanceID() == left.GetInstanceID()) return right;
            return left;
        }

        public bool AllowsTravel() {
            return direction switch {
                LinkDirection.Left => right.allocated,
                LinkDirection.Right => left.allocated,
                _ => left.allocated || right.allocated
            };
        }

        public bool IsMandatory(PassiveNode node) {
            return mandatory && GetSide(node).Flip() == direction;
        }

        public bool IsDependant(PassiveNode node) {
            return mandatory && GetSide(node) == direction;
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

