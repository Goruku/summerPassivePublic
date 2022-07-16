using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace Passive {
    [RequireComponent(typeof(PNode))]
    [ExecuteAlways]
    public class PLinkCaster : MonoBehaviour, ISerializationCallbackReceiver {

        public GameObject linkPrefab;

        [Serialize]
        public PLinkReceiver linkReceiver;
        private Dictionary<PNode, PLink> matchedLink = new ();
        [SerializeField, HideInInspector]
        private List<PNode> _matchedLinkKeys = new();
        [SerializeField, HideInInspector]
        private List<PLink> _matchedLinkValues = new();

        private PNode _casterNode;

        public int entry;


        private void Awake() {
            _casterNode = GetComponent<PNode>();

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
        void Update()
        {

        }

        private void OnEnable() {
            linkReceiver.passiveNodes.ItemAdded += AddNode;
            linkReceiver.passiveNodes.ItemRemoved += RemoveNode;
        }

        private void OnDisable() {
            linkReceiver.passiveNodes.ItemAdded -= AddNode;
            linkReceiver.passiveNodes.ItemRemoved -= RemoveNode;
        }

        private void AddNode(PNode node) {
            if (!node) return; 
            DestroyExistingNode(node);
            var link =GenerateLink(_casterNode, node, linkReceiver.linkContainer);
            matchedLink[node] = link;
            link.left.RegisterLink(link);
            link.right.RegisterLink(link);
        }

        private void RemoveNode(PNode node) {
            if (!node) return;
            DestroyExistingNode(node);
        }

        private void PassiveNodeModification(System.Object caller, NotifyCollectionChangedEventArgs eventArgs) {
            Debug.Log("Casting change");
            if (eventArgs.NewItems != null)
                foreach (PNode node in eventArgs.NewItems) {
                    if (!node) return; 
                    DestroyExistingNode(node);
                    var link =GenerateLink(_casterNode, node, linkReceiver.linkContainer);
                    matchedLink[node] = link;
                    link.left.RegisterLink(link);
                    link.right.RegisterLink(link);
                }
            if (eventArgs.OldItems != null)
                foreach (PNode node in eventArgs.OldItems) {
                    if (!node) return;
                    DestroyExistingNode(node);
                }
        }

        private void DestroyExistingNode(PNode node) {
            if (matchedLink.Remove(node, out PLink link)) {
                link.left.UnregisterLink(link);
                link.right.UnregisterLink(link);
                if (!Application.isPlaying)
                    DestroyImmediate(link.gameObject);
                else Destroy(link.gameObject);
            }
        }
        
        private PLink GenerateLink(PNode caster, PNode receivingNode, RectTransform linkHolder) {
            var link = Instantiate(linkPrefab, linkHolder).GetComponent<PLink>();
            link.left = caster;
            link.right = receivingNode;
            return link;
        }
    }
}
