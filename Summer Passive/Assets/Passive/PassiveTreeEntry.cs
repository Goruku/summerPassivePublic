using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace Passive {
    [RequireComponent(typeof(PassiveTree))]
    public class PassiveTreeEntry : MonoBehaviour {

        [Serialize]
        public ObservableList<PassiveNode> passiveNodes = new ();

        // Start is called before the first frame update
        void Start() {
            passiveNodes.ItemAdded += node => {
                if (node != null)
                    Debug.Log(node.name);
            };
        }

        // Update is called once per frame
        void Update()
        {
            passiveNodes.UpdateSerializationCallbacks();
        }
    }

    [Serializable]
    public class ObservableList<T> : IList<T>, INotifyCollectionChanged<T>, ISerializationCallbackReceiver {

        [SerializeField]
        private List<T> serializedListPreview = new();
        private List<T> _list = new ();
        
        private Queue<T> _changeAdded = new();
        private Queue<T> _changeRemoved = new();
        
        public event Action<T> ItemAdded = new Action<T>(obj => {});
        public event Action<T> ItemRemoved = new Action<T>(obj => {});
        public event Action CollectionChanged = new Action(() => { });

        public void OnBeforeSerialize() {
            serializedListPreview.Clear();
            serializedListPreview = new List<T>(_list);
        }
        
        public void OnAfterDeserialize() {

            bool changed = false;
            foreach (var item in _list) {
                if (!serializedListPreview.Contains(item)) {
                    _changeRemoved.Enqueue(item);
                    changed = true;
                }
            }
            foreach (var item in serializedListPreview) {
                if (!_list.Contains(item)) {
                    _changeAdded.Enqueue(item);
                }
                changed = true;
            }
            if (changed) CollectionChanged();
            _list = new List<T>(serializedListPreview);
        }

        public void UpdateSerializationCallbacks() {
            if (_changeAdded.Count <= 0 || _changeRemoved.Count <= 0) return ;
            
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

        public void Clear() { _list.Clear(); }

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
            get { return _list[index]; }
            set {
                _list[index] = value; 
                CollectionChanged.Invoke();
            }
        }
    }

}
