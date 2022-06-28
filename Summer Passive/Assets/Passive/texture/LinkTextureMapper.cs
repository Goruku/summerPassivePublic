using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passive.texture {
    [CreateAssetMenu(menuName = "passive/link/texture")]
    public class LinkTextureMapper : ScriptableObject {
        public Sprite nonDirected;
        public Sprite directed;

        public Color unavailable = Color.gray;
        public Color taken = new Color(255,215,0);
        public Color available = new Color(202, 135, 58);
        public Color mandates = Color.green;
        public Color mandated = Color.red;

        public Color GetColor(LinkState linkState) {
            return linkState switch {
                LinkState.Unavailable => unavailable,
                LinkState.Taken => taken,
                LinkState.Available => available,
                LinkState.Mandates => mandates,
                LinkState.Mandated => mandated,
                _ => UnityEngine.Color.black
            };
        }
    }
}
