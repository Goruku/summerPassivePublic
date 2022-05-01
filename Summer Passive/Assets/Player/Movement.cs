using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Player.Ability;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour {
    
    public float doubleTapWindow = 0.5f;

    public Dash dash;

    private KeyCode lastPressed = KeyCode.W;

    private KeyCode[] movementKeys = { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A };
    
    private readonly Dictionary<KeyCode, KeyInfo> _keyInfo= new ();

    // Start is called before the first frame update
    void Start() {
        foreach (KeyCode key in movementKeys) {
            _keyInfo.Add(key, new KeyInfo(0));
        }
    }

    // Update is called once per frame
    void Update() {
        foreach (KeyCode key in movementKeys) {
            DetectDashInput(key);
        }
    }

    private void FixedUpdate() {
        //General movement
        foreach (KeyCode key in movementKeys) {
            HeldKeyMove(key, 0.1f);
        }
        dash.Trigger();
    }
    
    private static Vector3 GetDirection(KeyCode keyCode) {
        return keyCode switch {
            KeyCode.W => Vector3.up,
            KeyCode.D => Vector3.right,
            KeyCode.S => Vector3.down,
            KeyCode.A => Vector3.left,
            _ => Vector3.zero
        };
    }

    private void HeldKeyMove(KeyCode keyCode, float magnitude) {
        if (Input.GetKey(keyCode))
            transform.position += GetDirection(keyCode) * magnitude;
    }

    private void DetectDashInput(KeyCode keyCode) {
        if (Input.GetKeyDown(keyCode)) {
            KeyInfo keyInfo = _keyInfo[keyCode];
            float currentTime = Time.time;
            if (currentTime - keyInfo.Timestamp <= doubleTapWindow && lastPressed == keyCode) {
                dash.direction = GetDirection(keyCode);
                dash.Arm();
            }
            keyInfo.Update();
            lastPressed = keyCode;
        }
    }
    
    private class KeyInfo {
        public float Timestamp;

        public KeyInfo(float time) {
            this.Timestamp = time;
        }

        public void Update() {
            Timestamp = Time.time;
        }
    }

}
