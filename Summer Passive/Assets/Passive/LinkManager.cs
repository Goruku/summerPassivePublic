using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Passive {
    [CustomEditor(typeof(LinkManager))]
    public class LinkManagerEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            LinkManager localTarget = (LinkManager)target;
            if (GUILayout.Button("Adjust Link Position")) {
                localTarget.Resize();
            }
        }
    }
    
    [ExecuteAlways]
    public class LinkManager : MonoBehaviour {
        public Canvas canvas;
        
        public List<PassiveLink> links;

        public bool autoResize;
        public int resizeRefreshRate = 20;

        private int _tickCount = 0;

        private bool _resetTickCount = false;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update() {
            ++_tickCount;
            if (autoResize && _tickCount >= resizeRefreshRate) {
                Resize();
                _resetTickCount = true;
            }

            if (_resetTickCount) {
                _tickCount = 0;
                _resetTickCount = false;
            }
        }

        public void Resize() {
            foreach (var link in links) {
                link.UpdateDimension();
            }
        }
    }
}

