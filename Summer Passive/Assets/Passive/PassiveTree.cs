using System.Collections.Generic;
using Passive;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(PassiveTree))]
public class LinkManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
            
        PassiveTree localTarget = (PassiveTree)target;
        if (GUILayout.Button("Update Tree")) {
            localTarget.UpdateLinks(true, true);
            localTarget.UpdateNodes(true);
        }
    }
}

[ExecuteAlways]
public class PassiveTree : MonoBehaviour, ISerializationCallbackReceiver {

    public Object linkPrefab;
    public GameObject linkContainer;
    
    public bool editLinks;
    public List<PassiveLinkRepresentation> _passiveLinks;

    public List<PassiveNode> _passiveNodes;

    private Dictionary<int, PassiveNode> passiveNodes = new ();
    
    private Dictionary<int, PassiveLinkRepresentation> passiveLinkRepresentations = new ();
    private Dictionary<int, PassiveLink> passiveLinks = new();
    
    public void OnBeforeSerialize() {
        _passiveNodes.Clear();
        foreach (var kvp in passiveNodes) {
            _passiveNodes.Add(kvp.Value);
        }

        _passiveLinks.Clear();
        foreach (var kvp in passiveLinkRepresentations) {
            _passiveLinks.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize() {
        passiveNodes = new Dictionary<int, PassiveNode>();
        int keyCount = 0;
        foreach (var passiveNode in _passiveNodes) {
            if (passiveNode != null) {
                passiveNode.id = keyCount;
            }
            passiveNodes.Add(keyCount, passiveNode); 
            keyCount++;
        }

        passiveLinkRepresentations = new Dictionary<int, PassiveLinkRepresentation>();
        int linkCount = 0;
        foreach (var passiveLink in _passiveLinks) {
            passiveLinkRepresentations.Add(linkCount, passiveLink);
            linkCount++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (editLinks) {
            
            foreach (Transform child in linkContainer.transform) {
                if (Application.isEditor) {
                    DestroyImmediate(child.gameObject);
                } else {
                    Destroy(child.gameObject);
                }
            }

            foreach (var passiveNode in _passiveNodes) {
                passiveNode.links.Clear();
            }
            passiveLinks.Clear();
            foreach (var kvp in passiveLinkRepresentations) {
                PassiveLink passiveLink = Instantiate(linkPrefab, linkContainer.transform).GetComponent<PassiveLink>();
                if (!passiveNodes.TryGetValue(kvp.Value.left, out passiveLink.left)) {
                    Debug.Log("Left node \" " + kvp.Value.left + " \"  of link " + kvp.Key + " couldn't be found");
                } else {
                    passiveLink.left.links.Add(passiveLink);
                }
                if (!passiveNodes.TryGetValue(kvp.Value.right, out passiveLink.right)) {
                    Debug.Log("Right node \" " + kvp.Value.right + " \"  of link " + kvp.Key + " couldn't be found");
                }
                else {
                    passiveLink.right.links.Add(passiveLink);
                }

                passiveLink.mandatory = kvp.Value.mandatory;
                passiveLink.linkState = kvp.Value.state;
                passiveLink.direction = kvp.Value.direction;
                passiveLinks.Add(kvp.Key, passiveLink);
                passiveLink.UpdateState();
                passiveLink.UpdateDimension();
            }
        }

        foreach (var passiveNode in _passiveNodes) {
            if (editLinks) {
                passiveNode.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = passiveNode.id.ToString();
            }
            else {
                passiveNode.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = passiveNode.text;
            }
        }
    }

    public void UpdateLinks(bool shouldAutoResize, bool shouldAutoLinkState) {
        foreach (var link in passiveLinks.Values) {
            if (shouldAutoResize)
                link.UpdateDimension();
            if (shouldAutoLinkState)
                link.UpdateState();
        }
    }

    public void UpdateNodes(bool shouldAutoPassiveNode) {
        foreach (var node in _passiveNodes) {
            if (shouldAutoPassiveNode)
                node.UpdateState();
        }
    }

    [System.Serializable]
    public struct PassiveLinkRepresentation {
        public int left;
        public int right;
        public LinkState state;
        public LinkDirection direction;
        public bool mandatory;

        public PassiveLinkRepresentation(int left, int right, LinkState state, LinkDirection direction,
            bool mandatory) {
            this.left = left;
            this.right = right;
            this.state = state;
            this.direction = direction;
            this.mandatory = mandatory;
        }
    }
}
