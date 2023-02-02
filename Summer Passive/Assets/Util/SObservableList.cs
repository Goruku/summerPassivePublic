using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Util {
    [Serializable]
    public class SObservableList<T> : IList<T>, INotifyCollectionChanged<T>
#if UNITY_EDITOR
        , ISerializationCallbackReceiver 
#endif
    {

#if UNITY_EDITOR
        [SerializeField]
        private List<T> serializedListInterface = new();
#endif

        [Serialize]
        private Queue<T> _changeAdded = new();
        [Serialize]
        private Queue<T> _changeRemoved = new();
        
        public event Action<T> ItemAdded = new Action<T>(obj => {});
        public event Action<T> ItemRemoved = new Action<T>(obj => {});
        public event Action CollectionChanged = new Action(() => { });
        
        public event Action<int, int> CountChanged = new Action<int, int>((oldCount, newCount) => {});
        
        [SerializeField, HideInInspector]
        private List<T> _items = new List<T>();

        private int _oldCount;
        
#if UNITY_EDITOR
        public void OnBeforeSerialize() {
            serializedListInterface.Clear();
            serializedListInterface = new List<T>(_items);
        }
        
        public void OnAfterDeserialize() {
            foreach (var item in this) {
                if (!serializedListInterface.Contains(item)) {
                    _changeRemoved.Enqueue(item);
                }
            }
            foreach (var item in serializedListInterface) {
                if (!_items.Contains(item)) {
                    _changeAdded.Enqueue(item);
                }
            }

            _oldCount = _items.Count;
            _items = new List<T>(serializedListInterface);
        }
#endif

        public void UpdateSerializationCallbacks() {
            if (_items.Count != _oldCount) {
                CountChanged(_oldCount, _items.Count);
                _oldCount = _items.Count;
            }
            if (_changeAdded.Count <= 0 && _changeRemoved.Count <= 0) return ;
            
            while (_changeAdded.Count > 0) {
                ItemAdded(_changeAdded.Dequeue());
            }
            while (_changeRemoved.Count > 0) {
                ItemRemoved(_changeRemoved.Dequeue());
            }
        }

        public T[] GetDiffAdded() {
            return _changeAdded.ToArray();
        }

        public T[] GetDiffRemoved() {
            return _changeRemoved.ToArray();
        }

        public void SetItemsNoCallback(List<T> list) {
            _items = list;
        }
        
        public void Add(T item) {
            _items.Add(item);
            ItemAdded(item);
            CollectionChanged.Invoke();
            CountChanged(Count - 1, Count);
        }

        public bool Remove(T item) {
            var result = _items.Remove(item);
            ItemRemoved(item);
            CollectionChanged.Invoke();
            CountChanged(Count + 1, Count);
            return result;
        }

        public void Insert(int index, T item) {
            _items.Insert(index, item);
            ItemAdded.Invoke(item);
            CollectionChanged.Invoke();
            CountChanged(Count - 1, Count);
        }

        public void RemoveAt(int index) {
            var item = _items[index];
            _items.RemoveAt(index);
            ItemRemoved(item);
            CollectionChanged.Invoke();
            CountChanged(Count + 1, Count);
        }

        public void Clear() {
            var oldCount = Count;
            foreach (var item in _items) {
                Remove(item);
            }
            CountChanged(oldCount, Count);
        }

        public bool Contains(T item) { return _items.Contains(item); }

        public void CopyTo(T[] array, int arrayIndex) { _items.CopyTo(array, arrayIndex); }

        public int Count { get => _items.Count; }

        //TODO: wtf
        public bool IsReadOnly { get => false; }

        public IEnumerator<T> GetEnumerator() { return _items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return _items.GetEnumerator(); }

        public int IndexOf(T item) { return _items.IndexOf(item); }

        public T this[int index]
        {
            get => _items[index];
            set {
                var item = _items[index];
                _items[index] = value;
                ItemRemoved(item);
                ItemAdded(value);
                CollectionChanged.Invoke();
            }
        }

        public void SetItemNoCallback(int i, T item) {
            _items[i] = item;
        }
    }
}