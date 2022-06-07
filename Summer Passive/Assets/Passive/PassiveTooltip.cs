using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PassiveTooltip : MonoBehaviour {

    public TextMeshProUGUI header;
    public TextMeshProUGUI description;

    public RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }
}
