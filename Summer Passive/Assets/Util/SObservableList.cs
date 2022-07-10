using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Util {
    [Serializable]
    public class SObservableList<T> : ObservableCollection<T>
#if UNITY_EDITOR
        , ISerializationCallbackReceiver 
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