using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Passive {
    [CustomEditor(typeof(TreeManager))]
    public class LinkManagerEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            TreeManager localTarget = (TreeManager)target;
            if (GUILayout.Button("Update Tree")) {
                localTarget.UpdateLinks(true, true);
                localTarget.UpdateNodes(true);
            }
        }
    }
    
    [ExecuteAlways]
    public class TreeManager : MonoBehaviour {
        public Canvas canvas;
        
        public List<PassiveLink> links;
        public List<PassiveNode> nodes;

        public bool autoResize;
        public int resizeRefreshRate = 20;
        public bool autoLinkState;
        public int linkStateRefreshRate = 100;
        public bool autoPassiveNodes;
        public int passiveNodeRefreshRate = 200;

        private int _tickCount = 0;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update() {
            ++_tickCount;
            bool shouldAutoResize = false;
            bool shouldAutoLinkState = false;
            bool shouldAutoPassiveNode = false;
            if (autoResize && _tickCount % resizeRefreshRate == 0) { shouldAutoResize = true; }
            if (autoLinkState && _tickCount % linkStateRefreshRate == 0) { shouldAutoLinkState = true; }

            if (autoPassiveNodes && _tickCount % passiveNodeRefreshRate == 0) { shouldAutoPassiveNode = true;}
            
            if (shouldAutoResize || shouldAutoLinkState)
                UpdateLinks(shouldAutoResize, shouldAutoLinkState);

            if (shouldAutoPassiveNode)
                UpdateNodes(shouldAutoPassiveNode);
        }

        public void UpdateLinks(bool shouldAutoResize, bool shouldAutoLinkState) {
            foreach (var link in links) {
                if (shouldAutoResize)
                    link.UpdateDimension();
                if (shouldAutoLinkState)
                    link.UpdateState();
            }
        }

        public void UpdateNodes(bool shouldAutoPassiveNode) {
            foreach (var node in nodes) {
                if (shouldAutoPassiveNode)
                    node.UpdateState();
            }
        }
    }
}

