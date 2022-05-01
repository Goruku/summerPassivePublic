using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour
{
    
    public float doubleTapWindow = 0.5f;
    public float dashCooldown = 2f;

    public float dashVelocity = 25f;

    private Vector3 dashDirection = Vector3.zero;
    private bool wantDash = false;
    private float lastDash = 0f;
    private KeyCode lastPressed = KeyCode.W;

    private KeyCode[] movementKeys = { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A };
    
    private KeyTimeStamp[] _timeStamps = {
        new (KeyCode.W, 0f),
        new (KeyCode.D, 0f),
        new (KeyCode.S, 0f),
        new (KeyCode.A, 0f)
    };

    // Start is called before the first frame update
    void Start() {
        
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
        if (wantDash) {
            Dash();
            wantDash = false;
        }
    }

    private void Dash() {
        transform.position += dashDirection * dashVelocity;
    }
    
    private Vector3 GetDirection(KeyCode keyCode) {
        switch (keyCode)
        {
            case KeyCode.W: return Vector3.up;
            case KeyCode.D: return Vector3.right;
            case KeyCode.S: return Vector3.down;
            case KeyCode.A: return Vector3.left;
            default: return Vector3.zero;
        }
    }

    private KeyTimeStamp GetTimeStamp(KeyCode keyCode) {
        switch (keyCode) {
            case KeyCode.W: return _timeStamps[0];
            case KeyCode.D: return _timeStamps[1];
            case KeyCode.S: return _timeStamps[2];
            case KeyCode.A: return _timeStamps[3];
            default: return _timeStamps[0];
        }
    }

    private void HeldKeyMove(KeyCode keyCode, float magnitude) {
        if (Input.GetKey(keyCode))
            transform.position += GetDirection(keyCode) * magnitude;
    }

    private void DetectDashInput(KeyCode keyCode) {
        if (Input.GetKeyDown(keyCode)) {
            float currentTime = Time.time;
            if (currentTime - GetTimeStamp(keyCode).Timestamp <= doubleTapWindow &&
                currentTime - lastDash >= dashCooldown && lastPressed == keyCode) {
                wantDash = true;
                dashDirection = GetDirection(keyCode);
                Debug.Log("Dash dectected");
                lastDash = currentTime;
            }
            GetTimeStamp(keyCode).Update();
            lastPressed = keyCode;
        }
    }
    
    private class KeyTimeStamp {
        public KeyCode Key;
        public float Timestamp;

        public KeyTimeStamp(KeyCode key, float time) {
            this.Key = key;
            this.Timestamp = time;
        }

        public void Update() {
            this.Timestamp = Time.time;
        }
    }

}
