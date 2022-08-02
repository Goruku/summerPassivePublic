using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;


namespace Passive {
    
    [ExecuteAlways]
    [RequireComponent(typeof(PLinkCaster))]
    public class PLinkCasterManager : MonoBehaviour {
        
        [SerializeField, GetSet("type")]
        private CasterManagerType _type;
        public CasterManagerType type {
            get => _type;
            set {
                _type = value;
                UpdateCaster(_linkCaster.casterNode.allocated);
            }
        }

        [SerializeField, GetSet("count")]
        private int _count;
        public int count {
            get => _count;
            set {
                _count = value;
                UpdateCaster(_linkCaster.casterNode.allocated);
            }
        }


        [SerializeField, GetSet("self")]
        private bool _self;
        public bool self {
            get => _self;
            set {
                _self = value;
                UpdateCaster(_linkCaster.casterNode.allocated);
            }
        }
        
        [SerializeField, HideInInspector, GetSet("activeActorCount")]
        private int _activeActorCount;
        private int activeActorCount {
            get => _activeActorCount;
            set {
                _activeActorCount = value;
                UpdateCaster(_linkCaster.casterNode.allocated);
            }
        }
        
        public SObservableList<PNode> actors = new ();

        [SerializeField, HideInInspector] private PLinkCaster _linkCaster;

        private void Awake() {
            _linkCaster = GetComponent<PLinkCaster>();
        }

        private void UpdateCaster(bool casterAllocated) {
            _linkCaster.active = (!self || casterAllocated) && type switch {
                CasterManagerType.IgnoreCount => true,
                CasterManagerType.Count => activeActorCount >= count,
                CasterManagerType.All => activeActorCount >= actors.Count,
                _ => false
            };
        }

        private void ListenToNode(bool allocated) {
            activeActorCount += allocated ? 1 : -1;
        }

        private void Update() {
            actors.UpdateSerializationCallbacks();
        }

        private void OnEnable() {
            actors.ItemAdded += RegisterActor;
            actors.ItemRemoved += UnregisterActor;
            _linkCaster.casterNode.NodeActions += UpdateCaster;
            foreach (var actor in actors) {
                RegisterActor(actor);
            }
        }
        
        private void OnDisable() {
            actors.ItemAdded -= RegisterActor;
            actors.ItemRemoved -= UnregisterActor;
            _linkCaster.casterNode.NodeActions -= UpdateCaster;
            foreach (var actor in actors) {
                UnregisterActor(actor);
            }
        }

        private void RegisterActor(PNode node) {
            if (!node) return;
            node.NodeActions += ListenToNode;
            if (node.allocated) activeActorCount += 1;
        }

        private void UnregisterActor(PNode node) {
            if (!node) return;
            node.NodeActions -= ListenToNode;
            if (node.allocated) activeActorCount -= 1;
        }

        [Serializable]
        public enum CasterManagerType {
            IgnoreCount, Count, All
        }
    }
}

