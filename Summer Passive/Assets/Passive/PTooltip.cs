using TMPro;
using UnityEngine;

namespace Passive {
    public class PTooltip : MonoBehaviour {

        public TextMeshProUGUI header;
        public TextMeshProUGUI description;

        public RectTransform rectTransform;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
