using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Passive;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using System.Linq;
using Util;
using Object = UnityEngine.Object;

[CustomEditor(typeof(PTree))]
public class LinkManagerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
            
        PTree localTarget = (PTree)target;
        if (GUILayout.Button("Force Update")) {
            localTarget.UpdateLinks();
            localTarget.UpdateNodes();
        }

        if (GUILayout.Button("Sort Link Interface")) {
            localTarget.SortLinkInterface();
        }
    }
}

[Serializable]
[ExecuteAlways]
public class PTree : MonoBehaviour, ISerializationCallbackReceiver {

    public Object linkPrefab;
    public Transform linkContainer;

    public bool editLinks;
    
    [Serialize]
    private Dictionary<PassiveLinkSetting, PLink> _links = new ();
    [SerializeField, HideInInspector]
    private List<PassiveLinkSetting> _linkKeys = new ();
    [SerializeField, HideInInspector]
    private List<PLink> _linkValues = new ();
    
    public SObservableList<PassiveLinkSetting> _passiveLinks;
    
    private bool _deserialized;

    public List<PNode> _passiveNodes;
    private Dictionary<int, PNode> passiveNodes = new ();

    private bool _enableOnce;
    
    public void OnBeforeSerialize() {
        _passiveNodes.Clear();
        foreach (var kvp in passiveNodes) {
            _passiveNodes.Add(kvp.Value);
        }
        _linkKeys.Clear();
        _linkValues.Clear();
        foreach (var kvp in _links) {
            _linkKeys.Add(kvp.Key);
            _linkValues.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize() {
        passiveNodes = new Dictionary<int, PNode>();
        int keyCount = 0;
        foreach (var passiveNode in _passiveNodes) {
            if (passiveNode != null) {
                passiveNode.id = keyCount;
            }
            passiveNodes.Add(keyCount, passiveNode); 
            keyCount++;
        }

        _links = new Dictionary<PassiveLinkSetting, PLink>();
        for (int i = 0; i < _linkKeys.Count && i < _linkValues.Count; i++) {
            _links.Add(_linkKeys[i], _linkValues[i]);
        }
        
        _deserialized = true;
    }

    // Start is called before the first frame update
    private void Start() {
        
    }

    private void OnEnable() {
        _passiveLinks.ItemAdded += AddLink;
        _passiveLinks.ItemRemoved += RemoveLink;
    }

    private void OnDisable() {
        _passiveLinks.ItemAdded -= AddLink;
        _passiveLinks.ItemRemoved -= RemoveLink;
    }

    private void AddLink(PassiveLinkSetting pls) {
        DestroyExistingLink(pls);
        var link = PushSettingsToLink(pls,
            PrefabUtility.InstantiatePrefab(linkPrefab, linkContainer.transform).GetComponent<PLink>());
        link.left.RegisterLink(link);
        link.right.RegisterLink(link);
        _links.Add(pls, link);
        PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
    }

    private void RemoveLink(PassiveLinkSetting pls) {
        if (_links.Remove(pls, out PLink link)) {
            link.left.UnregisterLink(link);
            link.right.UnregisterLink(link);
            if (!Application.isPlaying)
                DestroyImmediate(link.gameObject);
            else Destroy(link.gameObject);
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
    }

    private void DestroyExistingLink(PassiveLinkSetting pls) {
        if (_links.Remove(pls, out PLink link)) {
            if (!Application.isPlaying)
                DestroyImmediate(link.gameObject);
            else Destroy(link.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {

        if (!_deserialized) return;
        _passiveLinks.UpdateSerializationCallbacks();
        
        if (editLinks) {

            for (int i = 0; i < _passiveNodes.Count; i++) {
                _passiveNodes[i].gameObject.name = $"{_passiveNodes[i].passiveName} ({i})";
            }
        }
        
        foreach (var passiveNode in _passiveNodes) {
            var passiveUI = passiveNode.GetComponent<PNodeUI>();
            if (passiveUI) {
                passiveUI.overloadName = editLinks ? passiveNode.id.ToString() : "";
            }
            passiveNode.NodeActions(passiveNode.allocated);
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(passiveNode);
            #endif
        }
        _deserialized = false;
    }

    public void SortLinkInterface() {
        for (int i = 0; i < _passiveLinks.Count; i++) {
            var linkSetting = _passiveLinks[i];
            if (linkSetting.left > linkSetting.right) {
                _passiveLinks[i] = new PassiveLinkSetting(linkSetting.travels,
                    linkSetting.right,
                    linkSetting.left,
                    linkSetting.direction.Flip(),
                    linkSetting.mandatory);
            }
        }
        var sortedDict = from entry in _passiveLinks orderby entry.left, entry.right select entry;
        _passiveLinks.SetItemsNoCallback(sortedDict.ToList());
        UpdateLinks();
    }

    public void UpdateLinks() {
        foreach (var link in _links.Values) {
            link.UpdateDimension();
            link.UpdateState();
        }
    }

    public void UpdateNodes() {
        foreach (var node in passiveNodes.Values) {
            node.NodeActions(node.allocated);
        }
    }
    
    private PLink PushSettingsToLink(PassiveLinkSetting plr, PLink pLink) {
        if (!passiveNodes.TryGetValue(plr.left, out pLink.left))
            throw new NullReferenceException($"Left node \"{plr.left}\" couldn't be found");
        if (!passiveNodes.TryGetValue(plr.right, out pLink.right))
            throw new NullReferenceException($"Right node \"{plr.right}\" couldn't be found");
        
        pLink.travels = plr.travels;
        pLink.direction = plr.direction;
        pLink.mandatory = plr.mandatory;

        pLink.left.links.Add(pLink);
        pLink.right.links.Add(pLink);
        
        pLink.UpdateState();
        pLink.UpdateDimension();
        
        #if UNITY_EDITOR
        PrefabUtility.RecordPrefabInstancePropertyModifications(pLink);
        #endif

        return pLink;
    }

    private PLink PushSettingsToLink(PassiveLinkSetting plr, PLink pLink, String name) {
        pLink = PushSettingsToLink(plr, pLink);
        pLink.gameObject.name = name;
        return pLink;
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

        public bool Equals(PassiveLinkSetting obj) {
            return travels == obj.travels && left == obj.left && right == obj.right && direction.Equals(obj.direction)
                   && mandatory == obj.mandatory;
        }

        public override int GetHashCode() {
            return (travels, left, right, direction, mandatory).GetHashCode();
        }
    }

    [Serializable]
    public struct TreeLink {
        public PNode left;
        public PassiveLinkSetting linkSetting;
        public PNode right;
    }
}
