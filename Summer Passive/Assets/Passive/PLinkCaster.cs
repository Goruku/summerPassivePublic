using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Util;

namespace Passive {
    [RequireComponent(typeof(PNode))]
    [ExecuteAlways]
    public class PLinkCaster : MonoBehaviour, ISerializationCallbackReceiver {

        public GameObject linkPrefab;
        
        [SerializeField, GetSet("active")]
        private bool _active;
        public bool active {
            get { return _active;}
            set {
                _active = value;
                UpdateAllMaterialization();
            }
        }
        
        [Serialize]
        public SObservableList<PLinkReceiver> linkReceivers = new ();

        private Dictionary<PNode, PLink> matchedLink = new ();
        [SerializeField, HideInInspector]
        private List<PNode> _matchedLinkKeys = new();
        [SerializeField, HideInInspector]
        private List<PLink> _matchedLinkValues = new();

        [SerializeField, HideInInspector]
        private bool wasEnabled;

        [HideInInspector]
        public PNode casterNode;

        public int entry;


        private void Awake() {
            casterNode = GetComponent<PNode>();
        }

        public void OnBeforeSerialize() {
            _matchedLinkKeys.Clear();
            _matchedLinkValues.Clear();
            foreach (var kvp in matchedLink) {
                _matchedLinkKeys.Add(kvp.Key);
                _matchedLinkValues.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize() {
            matchedLink = new Dictionary<PNode, PLink>();
            for (int i = 0; i < _matchedLinkKeys.Count && i < _matchedLinkValues.Count; i++) {
                matchedLink.Add(_matchedLinkKeys[i], _matchedLinkValues[i]);
            }
        }

        private void OnDestroy() {
            
        }
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update() {
            linkReceivers.UpdateSerializationCallbacks();
        }

        private void OnEnable() {
            linkReceivers.ItemAdded += RegisterReceiver;
            linkReceivers.ItemRemoved += UnregisterReceiver;

            if (!wasEnabled)
                foreach (var linkReceiver in linkReceivers) {
                    if (!linkReceiver) continue;
                    if (linkReceiver.enabled)
                        RegisterReceiver(linkReceiver);
                }
            wasEnabled = true;
        }

        private void OnDisable() {
            linkReceivers.ItemAdded -= RegisterReceiver;
            linkReceivers.ItemRemoved -= UnregisterReceiver;

            if (wasEnabled)
                foreach (var linkReceiver in linkReceivers) {
                    if (!linkReceiver) continue;
                    UnregisterReceiver(linkReceiver);
                }
            wasEnabled = false;
        }

        private void HandleRegistrationFromEnable(bool rEnabled, PLinkReceiver linkReceiver) {
            if (rEnabled)
                RegisterReceiver(linkReceiver);
            else
                UnregisterReceiver(linkReceiver);
        }

        private void RegisterReceiver(PLinkReceiver linkReceiver) {
            if (!linkReceiver) return;
            linkReceiver.NodeAdded += AddNode;
            linkReceiver.passiveNodes.ItemRemoved += RemoveNode;
            linkReceiver.OnAbleChange += HandleRegistrationFromEnable;
            foreach (var node in linkReceiver.passiveNodes) {
                AddNode(node, linkReceiver);
            }
        }

        private void UnregisterReceiver(PLinkReceiver linkReceiver) {
            if (!linkReceiver) return;
            linkReceiver.NodeAdded -= AddNode;
            linkReceiver.passiveNodes.ItemRemoved -= RemoveNode;
            linkReceiver.OnAbleChange -= HandleRegistrationFromEnable;
            foreach (var node in linkReceiver.passiveNodes) {
                RemoveNode(node);
            }
        }

        private void UpdateAllMaterialization() {
            if (active) {
                MaterializeAllLinks();
            }
            else {
                UnmaterializeAllLinks();
            }
        }

        private void UpdateMaterialization(PLink link) {
            if (active) {
                MaterializeLink(link);
            }
            else {
                UnmaterializeLink(link);
            }
        }

        private void MaterializeAllLinks() {
            foreach (var link in matchedLink.Values) {
                MaterializeLink(link);
            }
        }
        
        private void UnmaterializeAllLinks() {
            foreach (var link in matchedLink.Values) {
                UnmaterializeLink(link);
            }
        }

        private void MaterializeLink(PLink link) {
            link.left.RegisterLink(link);
            link.right.RegisterLink(link);
            link.gameObject.SetActive(true);
            link.UpdateState();
            link.UpdateDimension();
        }
        
        private void UnmaterializeLink(PLink link) {
            link.left.UnregisterLink(link);
            link.right.UnregisterLink(link);
            link.gameObject.SetActive(false);
        }



        private void AddNode(PNode node, PLinkReceiver linkReceiver) {
            if (!node) return; 
            DestroyExistingNode(node);
            var link =GenerateLink(casterNode, node, linkReceiver.rectTransform);
            matchedLink[node] = link;
            UpdateMaterialization(link);
        }

        private void RemoveNode(PNode node) {
            if (!node) return;
            DestroyExistingNode(node);
        }

        private void DestroyExistingNode(PNode node) {
            if (matchedLink.Remove(node, out PLink link)) {
                if (!link) return;
                UnmaterializeLink(link);
                if (!Application.isPlaying)
                    DestroyImmediate(link.gameObject);
                else Destroy(link.gameObject);
            }
        }
        
        private PLink GenerateLink(PNode caster, PNode receivingNode, RectTransform linkHolder) {
            var link = PrefabUtility.InstantiatePrefab(linkPrefab, linkHolder).GetComponent<PLink>();
            link.name = $"Link (Caster {caster.id}, {receivingNode.id})";
            link.left = caster;
            link.right = receivingNode;
            return link;
        }
    }
}
