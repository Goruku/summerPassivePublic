using System.Collections.Generic;
using Player;
using Player.Ability;
using UnityEngine;

public class Movement : MonoBehaviour {

    public KeyManager keyManager;

    public Dash dash;

    private KeyCode lastPressed = KeyCode.W;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        foreach (KeyInfo keyInfo in keyManager.movementKeys) {
            if (keyInfo.doubleTap) {
                dash.direction = keyInfo.direction;
                dash.Arm();
            }
        }
    }

    private void FixedUpdate() {
        //General movement
        foreach (KeyInfo keyInfo in keyManager.movementKeys) {
            Move(keyInfo.held, keyInfo.direction, 0.1f);
        }
        dash.Trigger();
    }

    private void Move(bool condition, Vector3 direction, float magnitude) {
        if (condition)
            transform.position += direction * magnitude;
    }

}
