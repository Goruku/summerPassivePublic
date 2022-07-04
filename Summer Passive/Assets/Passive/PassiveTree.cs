using System;
using System.Collections.Generic;
using Passive;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using System.Linq;
using Object = UnityEngine.Object;

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
    public List<PassiveLinkSetting> _passiveLinks;
    public List<PassiveLink> linkPool = new ();
    private bool _linksChanged;

    private Dictionary<int, PassiveLinkSetting> passiveLinkRepresentations = new ();
    
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

        passiveLinkRepresentations = new Dictionary<int, PassiveLinkSetting>();
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

            ManageLinkPool();

            for (int i = 0; i < _passiveNodes.Count; i++) {
                _passiveNodes[i].links.Clear();
                if (_passiveNodes[i].HasCustomGraphics()) {
                    _passiveNodes[i].gameObject.name = $"{_passiveNodes[i].GetGraphics().nameText} ({i})";
                }
                else {
                    _passiveNodes[i].gameObject.name = $"({i})";
                }
                
            }
            for (int i = 0; i < _passiveLinks.Count; i++) {
                var passiveLink =  linkPool[i];
                PassiveLinkSetting plr = _passiveLinks[i];
                PushSettingsToLink(plr, passiveLink, $"({plr.left},{plr.right})");
            }
            _linksChanged = false;
        }

        foreach (var passiveNode in _passiveNodes) {
            if (passiveNode.HasCustomGraphics()) {
                if (editLinks) {
                    passiveNode.GetGraphics().SetText(passiveNode.id.ToString());
                } else {
                    passiveNode.GetGraphics().SetText(passiveNode.GetGraphics().nameText);
                }
            }
            #if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(passiveNode);
            #endif
        }
    }

    private void ManageLinkPool() {
        if (linkPool.Count != linkContainer.childCount) {
            linkPool.Clear();
            for (var i = linkContainer.childCount - 1; i >= 0; i--) {
                if (Application.isPlaying)
                    Destroy(linkContainer.GetChild(i).gameObject);
                else
                    DestroyImmediate(linkContainer.GetChild(i).gameObject);
            }
        }
        while (linkPool.Count != _passiveLinks.Count) {
            if (linkPool.Count < _passiveLinks.Count) {
                #if UNITY_EDITOR
                if (!Application.isPlaying) { 
                    linkPool.Add(PrefabUtility.InstantiatePrefab(linkPrefab, linkContainer.transform).GetComponent<PassiveLink>());
                } else
                #endif
                linkPool.Add(Instantiate(linkPrefab, linkContainer.transform).GetComponent<PassiveLink>());
            }
            else {
                var link = linkPool[0];
                linkPool.RemoveAt(0);
                if (Application.isPlaying)
                    Destroy(link.gameObject);
                else
                    DestroyImmediate(link);
            }
        }
    }

    public void SortLinkInterface() {
        for (int i = 0; i < _passiveLinks.Count; i++) {
            PassiveLinkSetting linkSetting = _passiveLinks[i];
            if (linkSetting.left > linkSetting.right) {
                passiveLinkRepresentations[i] = new PassiveLinkSetting(linkSetting.travels,
                    linkSetting.right,
                    linkSetting.left,
                    linkSetting.direction.Flip(),
                    linkSetting.mandatory);
            }
        }
        var sortedDict = from entry in passiveLinkRepresentations orderby entry.Value.left, entry.Value.right select entry;
        passiveLinkRepresentations = sortedDict.ToDictionary(t => t.Key, t => t.Value);
        UpdateLinks();
    }

    public void UpdateLinks() {
        foreach (var link in linkPool) {
            link.UpdateDimension();
            link.UpdateState();
        }
    }

    public void UpdateNodes() {
        foreach (var node in passiveNodes.Values) {
            node.UpdateState();
        }
    }
    
    private PassiveLink PushSettingsToLink(PassiveLinkSetting plr, PassiveLink passiveLink) {
        if (!passiveNodes.TryGetValue(plr.left, out passiveLink.left))
            throw new NullReferenceException($"Left node \"{plr.left}\" couldn't be found");
        if (!passiveNodes.TryGetValue(plr.right, out passiveLink.right))
            throw new NullReferenceException($"Right node \"{plr.right}\" couldn't be found");
        
        passiveLink.travels = plr.travels;
        passiveLink.direction = plr.direction;
        passiveLink.mandatory = plr.mandatory;

        passiveLink.left.links.Add(passiveLink);
        passiveLink.right.links.Add(passiveLink);
        
        passiveLink.UpdateState();
        passiveLink.UpdateDimension();
        
        #if UNITY_EDITOR
        PrefabUtility.RecordPrefabInstancePropertyModifications(passiveLink);
        #endif

        return passiveLink;
    }

    private PassiveLink PushSettingsToLink(PassiveLinkSetting plr, PassiveLink passiveLink, String name) {
        passiveLink = PushSettingsToLink(plr, passiveLink);
        passiveLink.gameObject.name = name;
        return passiveLink;
    }

    [Serializable]
    public struct PassiveLinkSetting {
        public bool travels;
        public int left;
        public int right;
        public LinkDirection direction;
        public bool mandatory;

        public PassiveLinkSetting(bool travels, int left, int right, LinkDirection direction,
            bool mandatory) {
            this.travels = travels;
            this.left = left;
            this.right = right;
            this.direction = direction;
            this.mandatory = mandatory;
        }
    }

    [Serializable]
    public struct TreeLink {
        public PassiveNode left;
        public PassiveLinkSetting linkSetting;
        public PassiveNode right;
    }
}
