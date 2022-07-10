using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Util;

namespace Passive {
    [RequireComponent(typeof(PassiveTree))]
    [ExecuteAlways]
    public class PassiveTreeEntry : MonoBehaviour {

        private bool enableOnce;

        [SerializeField, GetSet("active")]
        private bool _active;

        public bool active {
            get => _active;
            set {
                _active = value;
                if (!enabled) return;
                foreach (var node in passiveNodes) {
                    node.neighbourNeeded += _active ? -1 : 1;
                }
            }
        }

        [Serialize]
        public ObservableList<PassiveNode> passiveNodes = new ();

        private void ListenToChange(object caller, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null)
                foreach (PassiveNode node in e.NewItems) {
                    if (!node) continue;
                    SwitchOnWithActive(node, enabled);
                }
            if (e.OldItems != null)
                foreach (PassiveNode node in e.OldItems) {
                    if (!node) continue;
                    SwitchOffWithActive(node, enabled);
                }
        }

        private void OnEnable() {
            if (enableOnce) return;
            passiveNodes.CollectionChanged += ListenToChange;
            foreach (var node in passiveNodes) {
                SwitchOnWithActive(node);
            }
            
            enableOnce = true;
        }

        private void OnDisable() {
            if (!enableOnce) return;
            passiveNodes.CollectionChanged -= ListenToChange;
            foreach (var node in passiveNodes) {
                SwitchOffWithActive(node);
            }

            enableOnce = false;
        }
        
        void Update()
        {
            passiveNodes.UpdateSerializationCallbacks();
        }

        private void SwitchOnWithActive(PassiveNode node, bool guardFalse = true) {
            if (!guardFalse) return;
            node.neighbourNeeded += active ? -1 : 0;
        }
        
        private void SwitchOffWithActive(PassiveNode node, bool guardFalse = true) {
            if (!guardFalse) return;
            node.neighbourNeeded += active ? 1 : 0;
        }
    }

    [Serializable]
    public class ObservableList<T> : ObservableCollection<T>, 
        #if UNITY_EDITOR
        ISerializationCallbackReceiver 
        #endif
        {

        #if UNITY_EDITOR
        [SerializeField]
        private List<T> serializedListInterface = new();
        #endif
            
            
            
        private Queue<T> _changeAdded = new();
        private Queue<T> _changeRemoved = new();

        private IList<T> _items;
        protected new IList<T> Items {
            get => _items;
            set => _items = value;
        }
        #if UNITY_EDITOR
        public void OnBeforeSerialize() {
            serializedListInterface.Clear();
            serializedListInterface = new List<T>(Items);
        }
        
        public void OnAfterDeserialize() {
            
            foreach (var item in this) {
                if (!serializedListInterface.Contains(item)) {
                    _changeRemoved.Enqueue(item);
                }
            }
            foreach (var item in serializedListInterface) {
                if (!Contains(item)) {
                    _changeAdded.Enqueue(item);
                }
            }
            Items = new List<T>(serializedListInterface);
        }
        #endif

        public void UpdateSerializationCallbacks() {
            if (_changeAdded.Count <= 0 && _changeRemoved.Count <= 0) return ;
            
            while (_changeAdded.Count > 0) {
                Add(_changeAdded.Dequeue());
            }
            while (_changeRemoved.Count > 0) {
                Remove(_changeRemoved.Dequeue());
            }
        }

        public T[] GetDiffAdded() {
            return _changeAdded.ToArray();
        }

        public T[] GetDiffRemoved() {
            return _changeRemoved.ToArray();
        }
    }

}
