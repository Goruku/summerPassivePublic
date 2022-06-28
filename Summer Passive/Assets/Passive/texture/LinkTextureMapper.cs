using UnityEngine;

namespace Passive.texture {
    [CreateAssetMenu(menuName = "passive/link/texture")]
    public class LinkTextureMapper : ScriptableObject {
        public Sprite nonDirected;
        public Sprite directed;
        public Sprite nonDirectedTL;
        public Sprite directedTL;

        public float nonDirectedAlpha = 0.5f;

        public Color unavailable = Color.gray;
        public Color taken = new Color(255,215,0);
        public Color available = new Color(202, 135, 58);
        public Color mandates = Color.green;
        public Color mandated = Color.red;

        public Color GetColor(LinkState linkState, bool travels) {
            return linkState switch {
                LinkState.Unavailable => ApplyAlpha(unavailable, travels),
                LinkState.Taken => ApplyAlpha(taken, travels),
                LinkState.Available => ApplyAlpha(available, travels),
                LinkState.Mandates => ApplyAlpha(mandates, travels),
                LinkState.Mandated => ApplyAlpha(mandated, travels),
                _ => ApplyAlpha(Color.black, travels)
            };
        }

        private Color ApplyAlpha(Color color, bool travels) {
            if (travels)
                return color;
            return color * new Color(1,1,1,nonDirectedAlpha);
        }

        public Sprite GetSprite(LinkDirection direction, bool travels) {
            if (travels)
                return direction == LinkDirection.None ? nonDirected : directed;
            return direction == LinkDirection.None ? nonDirectedTL : directedTL;
        }
    }
}
