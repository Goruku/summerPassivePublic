using System.Collections.Generic;
using Player;
using Player.Ability;
using UnityEngine;

public class Movement : MonoBehaviour {
    
    public float doubleTapWindow = 0.5f;

    public Dash dash;

    private KeyCode lastPressed = KeyCode.W;

    private KeyCode[] movementKeys = { KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A };
    
    private readonly Dictionary<KeyCode, ControlUtility.KeyInfo> _keyInfo= new ();

    // Start is called before the first frame update
    void Start() {
        foreach (KeyCode key in movementKeys) {
            _keyInfo.Add(key, new ControlUtility.KeyInfo(0));
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

    private void HeldKeyMove(KeyCode keyCode, float magnitude) {
        if (Input.GetKey(keyCode))
            transform.position += ControlUtility.GetDirection(keyCode) * magnitude;
    }

    private void DetectDashInput(KeyCode keyCode) {
        if (Input.GetKeyDown(keyCode)) {
            ControlUtility.KeyInfo keyInfo = _keyInfo[keyCode];
            float currentTime = Time.time;
            if (currentTime - keyInfo.Timestamp <= doubleTapWindow && lastPressed == keyCode) {
                dash.direction = ControlUtility.GetDirection(keyCode);
                dash.Arm();
            }
            keyInfo.Update();
            lastPressed = keyCode;
        }
    }

}
