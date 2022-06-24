using System.Collections.Generic;
using Passive;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

[CustomEditor(typeof(PassiveTree))]
public class LinkManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
            
        PassiveTree localTarget = (PassiveTree)target;
        if (GUILayout.Button("Force Update")) {
            localTarget.UpdateLinks();
            localTarget.UpdateNodes();
        }

        if (GUILayout.Button("Sort Link Interface")) {
            localTarget.SortLinkInterface();
        }
    }
}

[ExecuteAlways]
public class PassiveTree : MonoBehaviour, ISerializationCallbackReceiver {

    public Object linkPrefab;
    public Transform linkContainer;
    
    public bool editLinks;
    public List<PassiveLinkRepresentation> _passiveLinks;
    private Dictionary<int, PassiveLink> passiveLinks = new();
    private bool _linksChanged;

    private Dictionary<int, PassiveLinkRepresentation> passiveLinkRepresentations = new ();
    
    public List<PassiveNode> _passiveNodes;
    private Dictionary<int, PassiveNode> passiveNodes = new ();
    
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
        _linksChanged = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_linksChanged && editLinks) {

            linkContainer = transform.GetChild(0);

            while (linkContainer.childCount != _passiveLinks.Count) {
                if (linkContainer.childCount < _passiveLinks.Count) {
                    var passiveLink = Instantiate(linkPrefab, linkContainer.transform).GetComponent<PassiveLink>();
                }
                else {
                    if (Application.isEditor) {
                        DestroyImmediate(linkContainer.GetChild(0).gameObject);
                    }
                    else {
                        Destroy(linkContainer.GetChild(0).gameObject);
                    }
                }
            }

            for (int i = 0; i < _passiveNodes.Count; i++) {
                _passiveNodes[i].links.Clear();
                _passiveNodes[i].gameObject.name = _passiveNodes[i].nameText + " (" + i + ")";
            }
            for (int i = 0; i < _passiveLinks.Count; i++) {
                PassiveLink passiveLink =  linkContainer.GetChild(i).GetComponent<PassiveLink>();
                PassiveLinkRepresentation linkRepresentation = _passiveLinks[i];
                passiveLink.gameObject.name = i.ToString();
                if (!passiveNodes.TryGetValue(linkRepresentation.left, out passiveLink.left)) {
                    Debug.Log("Left node \" " + linkRepresentation.left + " \"  of link " + i + " couldn't be found");
                } else {
                    passiveLink.left.links.Add(passiveLink);
                }
                if (!passiveNodes.TryGetValue(_passiveLinks[i].right, out passiveLink.right)) {
                    Debug.Log("Right node \" " + linkRepresentation.right + " \"  of link " + i + " couldn't be found");
                }else {
                    passiveLink.right.links.Add(passiveLink);
                }
                
                passiveLink.mandatory = linkRepresentation.mandatory;
                passiveLink.linkState = linkRepresentation.state;
                passiveLink.direction = linkRepresentation.direction;
                passiveLink.LinkComponents();
                passiveLink.UpdateState();
                passiveLink.UpdateDimension();
            }
            _linksChanged = false;
        }

        foreach (var passiveNode in _passiveNodes) {
            if (editLinks) {
                passiveNode.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = passiveNode.id.ToString();
                #if UNITY_EDITOR
                //PrefabUtility.RecordPrefabInstancePropertyModifications(passiveNode);
                #endif
            }
            else {
                passiveNode.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = passiveNode.nameText;
            }
        }
    }

    public void SortLinkInterface() {
        for (int i = 0; i < _passiveLinks.Count; i++) {
            PassiveLinkRepresentation linkRepresentation = _passiveLinks[i];
            if (linkRepresentation.left > linkRepresentation.right) {
                passiveLinkRepresentations[i] = new PassiveLinkRepresentation(linkRepresentation.right,
                    linkRepresentation.left,
                    linkRepresentation.state,
                    linkRepresentation.direction.Flip(),
                    linkRepresentation.mandatory);
            }
        }
        var sortedDict = from entry in passiveLinkRepresentations orderby entry.Value.left, entry.Value.right select entry;
        passiveLinkRepresentations = sortedDict.ToDictionary(t => t.Key, t => t.Value);
    }

    public void UpdateLinks() {
        foreach (var link in passiveLinks.Values) {
            link.UpdateDimension();
            link.UpdateState();
        }
    }

    public void UpdateNodes() {
        foreach (var node in passiveNodes.Values) {
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
