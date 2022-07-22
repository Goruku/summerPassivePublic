using System;
using Passive;
using Unity.VisualScripting;
using UnityEngine;
using Util;

[ExecuteAlways]
public class PLinkReceiver : MonoBehaviour {

    public int entry;

    [Serialize]
    public SObservableList<PNode> passiveNodes = new ();
    public ReceiverDelegateInterface NodeAdded = (node, receiver) => {};
    public ReceiverDelegateInterface NodeRemoved = (node, receiver) => {};
    public ReceiverEnabled OnAbleChange = (abled, receiver) => { };
    [HideInInspector]
    public RectTransform rectTransform;

    public delegate void ReceiverDelegateInterface(PNode node, PLinkReceiver receiver);

    public delegate void ReceiverEnabled(bool enabled, PLinkReceiver receiver);

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable() {
        passiveNodes.ItemAdded += callAdd;
        passiveNodes.ItemRemoved += callRemove;
        OnAbleChange(true, this);
    }
    
    private void OnDisable() {
        passiveNodes.ItemAdded -= callAdd;
        passiveNodes.ItemRemoved -= callRemove;
        OnAbleChange(false, this);
    }

    private void callAdd(PNode node) {
        NodeAdded(node, this);
    }

    private void callRemove(PNode node) {
        NodeRemoved(node, this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        passiveNodes.UpdateSerializationCallbacks();
    }
}
