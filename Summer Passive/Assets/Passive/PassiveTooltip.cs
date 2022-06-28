using TMPro;
using UnityEngine;

public class PassiveTooltip : MonoBehaviour {

    public TextMeshProUGUI header;
    public TextMeshProUGUI description;

    public RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }
}
