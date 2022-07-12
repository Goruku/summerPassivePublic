using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Passive;
using UnityEngine;
using Util;

[ExecuteAlways]
public class PLinkReceiver : MonoBehaviour {

    public int entry;

    public SObservableList<PNode> passiveNodes = new ();
    public RectTransform linkContainer;

    private void Awake() {
        if (linkContainer == null) {
            linkContainer = new GameObject($"linkReceiver ({entry})", typeof(RectTransform)).GetComponent<RectTransform>();
            linkContainer.SetParent(transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
