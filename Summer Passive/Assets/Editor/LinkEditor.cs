using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LinkCreator))]
public class LinkEditor : Editor {
    LinkCreator creator;

    private void OnSceneGUI() {
        if (creator.autoUpdate && Event.current.type == EventType.Repaint) {
            creator.UpdateLink();
        }
    }

    private void OnEnable() {
        creator = (LinkCreator)target;
    }
}
