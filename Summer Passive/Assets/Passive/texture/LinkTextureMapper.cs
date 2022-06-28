using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Passive.texture {
    [CreateAssetMenu(menuName = "passive/link/texture")]
    public class LinkTextureMapper : ScriptableObject {
        public Sprite nonDirected;
        public Sprite directed;
    }
}
