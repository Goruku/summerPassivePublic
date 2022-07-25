using System;
using Unity.VisualScripting;
using UnityEngine;
using Util;

namespace  Passive {
    
    [ExecuteAlways]
    public class PLinkReceiverGroup : MonoBehaviour {
    
        [Serialize]
        public SObservableList<PLinkReceiver> linkReceivers;

        private void Awake() {

        }

        private void OnEnable() {
            linkReceivers.CountChanged += InstantiateLinkReceiver;
            linkReceivers.ItemRemoved += DeleteLinkReceiver;
        }

        private void OnDisable() {
            linkReceivers.CountChanged -= InstantiateLinkReceiver;
            linkReceivers.ItemRemoved -= DeleteLinkReceiver;
        }

        private void InstantiateLinkReceiver(int oldCount, int count) {
            if (count <= oldCount) return;
            var linkContainer = new GameObject($"linkReceiver", typeof(RectTransform)).GetComponent<RectTransform>();
            linkContainer.SetParent(transform);
            var receiver = linkContainer.AddComponent<PLinkReceiver>();
            receiver.name = $"linkReceiver ({count - 1})";
            receiver.entry = count - 1;
            linkReceivers.SetItemNoCallback(count - 1, receiver);
        }

        private void DeleteLinkReceiver(PLinkReceiver linkReceiver) {
            if (!linkReceiver) return;
            linkReceiver.enabled = false;
            if (!Application.isPlaying)
                DestroyImmediate(linkReceiver.gameObject);
            else Destroy(linkReceiver.gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            linkReceivers.UpdateSerializationCallbacks();
        }
    }
}