using System;
using Passive.texture;
using UnityEngine;
using UnityEngine.UI;

namespace Passive {
    public class PLink : MonoBehaviour {

        public LinkTextureMapper linkTextureMapper;
        public bool travels = true;
        public PNode left;
        public PNode right;
        public LinkState linkState;
        public LinkDirection direction;
        public bool mandatory;

        public Image image;
        public RectTransform rectTransform;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() { }

        private void Awake() {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void UpdateState() {
            linkState = ComputeState();
            image.color = linkTextureMapper.GetColor(linkState, travels);
            image.sprite = linkTextureMapper.GetSprite(direction, travels);
        }

        public void UpdateDimension() {
            var leftPosition = left.GetRectTransform().anchoredPosition3D;
            var rightPosition = right.GetRectTransform().anchoredPosition3D;
            rectTransform.position = (rightPosition - leftPosition) * 0.5f;
            rectTransform.rotation = Quaternion.Euler(0, 0, MathF.Atan2(rightPosition.y - leftPosition.y, rightPosition.x - leftPosition.x) * Mathf.Rad2Deg);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 10);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                Vector3.Distance(rightPosition, leftPosition));
            rectTransform.anchoredPosition3D = leftPosition + (rightPosition - leftPosition) * 0.5f;
            rectTransform.localScale = direction == LinkDirection.Left ?  new Vector3(-1, 1, 1) :
                new Vector3(1, 1, 1);

        }
        
        private LinkState ComputeState() {
            if (left.allocated && right.allocated) return mandatory ? LinkState.Mandates : LinkState.Taken;
            if (HasParentTaken()) {
                return mandatory ? LinkState.Mandates : travels ? LinkState.Available : LinkState.Unavailable;
            }
            return mandatory ?  LinkState.Mandated : LinkState.Unavailable;
        }

        public LinkDirection GetSide(PNode node) {
            if (node.GetInstanceID() == left.GetInstanceID()) return LinkDirection.Left;
            return LinkDirection.Right;
        }

        public PNode GetLinkedPoint(PNode node) {
            if (node.GetInstanceID() == left.GetInstanceID()) return right;
            return left;
        }
        
        public bool HasParentTaken() {
            return direction switch {
                LinkDirection.Left => right.allocated,
                LinkDirection.Right => left.allocated,
                _ => left.allocated || right.allocated
            };
        }
        public bool AllowsTravel() {
            return travels && HasParentTaken();
        }

        public bool IsMandatory(PNode node) {
            return mandatory && GetSide(node).Flip() == direction;
        }

        public bool IsDependant(PNode node) {
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

