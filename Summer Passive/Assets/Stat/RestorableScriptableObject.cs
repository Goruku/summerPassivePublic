using UnityEditor;
using UnityEngine;

//Graciously offered by PluginMaster in their Discord Server https://assetstore.unity.com/publishers/48595
public abstract class RestorableScriptableObject : ScriptableObject {
    [SerializeField] public bool restorePlayMode = true;
    private SerializedObject _initialSerializedObject;

    protected RestorableScriptableObject() {
        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged += ManagePlayState;
        #endif
    }

    #if UNITY_EDITOR
    private void ManagePlayState(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredPlayMode) _initialSerializedObject = new SerializedObject(this);
        if (restorePlayMode && state == PlayModeStateChange.ExitingPlayMode) {
            var serializedObj = new SerializedObject(this);
            var prop = _initialSerializedObject.GetIterator();
            while (prop.NextVisible(true)) {
                if(prop.name != "_restorePlayMode") serializedObj.CopyFromSerializedProperty(prop);
            }
            serializedObj.ApplyModifiedProperties();
        }
        CleanUpPlayState();
    }
    #endif

    protected abstract void CleanUpPlayState();
}
