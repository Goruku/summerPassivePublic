using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Util;

namespace Passive {
    [RequireComponent(typeof(PassiveTree))]
    [ExecuteAlways]
    public class PassiveTreeEntry : MonoBehaviour {

        [SerializeField, GetSet("active")]
        private bool _active;

        public bool active {
            get => _active;
            set {
                _active = value;
                if (!enabled) return;
                foreach (var node in passiveNodes) {
                    node.neighbourNeeded += _active ? -1 : 1;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(node);
                }
            }
        }

        [Serialize]
        public ObservableList<PassiveNode> passiveNodes = new ();
        
        void Start() {
            passiveNodes.ItemAdded += node => {
                SwitchOnWithActive(node, enabled);
            };
            passiveNodes.ItemRemoved += node => {
                SwitchOffWithActive(node, enabled);
            };
        }

        private void OnEnable() {
            foreach (var node in passiveNodes) {
                SwitchOnWithActive(node);
            }
        }

        private void OnDisable() {
            foreach (var node in passiveNodes) {
                SwitchOffWithActive(node);
            }
        }
        
        void Update()
        {
            passiveNodes.UpdateSerializationCallbacks();
        }

        private void SwitchOnWithActive(PassiveNode node, bool guardFalse = true) {
            if (!guardFalse) return;
            node.neighbourNeeded += active ? -1 : 0;
            PrefabUtility.RecordPrefabInstancePropertyModifications(node);
        }
        
        private void SwitchOffWithActive(PassiveNode node, bool guardFalse = true) {
            if (!guardFalse) return;
            node.neighbourNeeded += active ? 1 : 0;
            PrefabUtility.RecordPrefabInstancePropertyModifications(node);
        }
    }

    [Serializable]
    public class ObservableList<T> : IList<T>, INotifyCollectionChanged<T>, 
        #if UNITY_EDITOR
        ISerializationCallbackReceiver 
        #endif
        {

        #if UNITY_EDITOR
        [SerializeField]
        private List<T> serializedListInterface = new();
        #endif
        private List<T> _list = new ();
        
        private Queue<T> _changeAdded = new();
        private Queue<T> _changeRemoved = new();
        
        public event Action<T> ItemAdded = new Action<T>(obj => {});
        public event Action<T> ItemRemoved = new Action<T>(obj => {});
        public event Action CollectionChanged = new Action(() => { });

        
        #if UNITY_EDITOR
        public void OnBeforeSerialize() {
            serializedListInterface.Clear();
            serializedListInterface = new List<T>(_list);
        }
        
        public void OnAfterDeserialize() {
            
            foreach (var item in _list) {
                if (!serializedListInterface.Contains(item)) {
                    _changeRemoved.Enqueue(item);
                }
            }
            foreach (var item in serializedListInterface) {
                if (!_list.Contains(item)) {
                    _changeAdded.Enqueue(item);
                }
            }
            _list = new List<T>(serializedListInterface);
        }
        #endif

        public void UpdateSerializationCallbacks() {
            if (_changeAdded.Count <= 0 && _changeRemoved.Count <= 0) return ;
            
            while (_changeAdded.Count > 0) {
                ItemAdded(_changeAdded.Dequeue());
            }
            while (_changeRemoved.Count > 0) {
                ItemRemoved(_changeRemoved.Dequeue());
            }
            CollectionChanged();
        }

        public T[] GetDiffAdded() {
            return _changeAdded.ToArray();
        }

        public T[] GetDiffRemoved() {
            return _changeRemoved.ToArray();
        }

        public void Add(T item) {
            _list.Add(item);
            ItemAdded(item);
            CollectionChanged.Invoke();
        }

        public bool Remove(T item) {
            var result = _list.Remove(item);
            ItemRemoved(item);
            CollectionChanged.Invoke();
            return result;
        }

        public void Insert(int index, T item) {
            _list.Insert(index, item);
            ItemAdded.Invoke(item);
            CollectionChanged.Invoke();
        }

        public void RemoveAt(int index) {
            var item = _list[index];
            _list.RemoveAt(index);
            ItemRemoved(item);
            CollectionChanged.Invoke();
        }

        public void Clear() {
            foreach (var item in _list) {
                Remove(item);
            }
        }

        public bool Contains(T item) { return _list.Contains(item); }

        public void CopyTo(T[] array, int arrayIndex) { _list.CopyTo(array, arrayIndex); }

        public int Count { get => _list.Count; }

        //TODO: wtf
        public bool IsReadOnly { get => false; }

        public IEnumerator<T> GetEnumerator() { return _list.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _list.GetEnumerator(); }

        public int IndexOf(T item) { return _list.IndexOf(item); }

        public T this[int index]
        {
            get => _list[index];
            set {
                var item = _list[index];
                _list[index] = value;
                ItemRemoved(item);
                ItemAdded(value);
                CollectionChanged.Invoke();
            }
        }
    }

}
