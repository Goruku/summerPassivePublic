using System.Collections.Generic;
using System.Linq;
using Player;
using Player.Ability;
using UnityEngine;

public class Movement : MonoBehaviour {

    public KeyManager keyManager;

    public Dash dash;

    public float moveSpeed;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        foreach (KeyInfo keyInfo in keyManager.movementKeys) {
            if (keyInfo.doubleTap || (keyManager.movementCombo.held && keyInfo.held)) {
                dash.shiftDirection(keyInfo.direction);
                dash.Arm();
            }
        }
    }

    private void FixedUpdate() {
        //General movement
        foreach (var keyInfo in keyManager.movementKeys) {
            Move(keyInfo.held, keyInfo.direction, moveSpeed);
        }
        dash.Trigger();
    }

    private void Move(bool condition, Vector3 direction, float magnitude) {
        if (condition)
            transform.position += direction * magnitude;
    }

}
