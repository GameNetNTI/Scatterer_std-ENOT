using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {
    public float speed = 2;

    private void Update() {
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, -speed * Time.deltaTime, 0);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
