using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour {
    private Vector3 MovingVector = new Vector3(0, 0, 0);
    private bool IsMoving = false;

    public float StartYPosition = 17;
    public float Xshift = 1000;
    public float stiffness1 = 1;
    public float stiffness2 = 1;
    public float stiffnessPow = 2;
    public float XMovingMP;
    public float ZMovingMP;
    public float Sensivity1 = 300;
    public float Sensivity2 = 300;
    public float startSpeed = 10f;
    public float speed = 10f;
    private float koeff = 0;
    private GameObject CameraClass;
    private Transform CameraTransform;

    private void Start() {
        CameraClass = gameObject.transform.Find("Main Camera").gameObject;
        CameraTransform = CameraClass.GetComponent<Transform>();
    }

    private void Update() {
        //float camRoof = CameraClass.GetComponent<CameraScrolling>().roof;
        //float camFloor = CameraClass.GetComponent<CameraScrolling>().floor;
        //float Y = CameraTransform.position.y;
        //koeff = (myroof - stiffness / (Y + myroof / stiffness - StartYPosition) + StartYPosition) * speed * Time.deltaTime;
        //koeff = (Mathf.Sqrt(CameraTransform.position.y/stiffness + startSpeed) + StartYPosition) * speed * Time.deltaTime;
        //koeff = speed * Time.deltaTime;
        koeff = (Mathf.Pow(CameraTransform.position.y - Xshift  * stiffness1, stiffnessPow) / stiffness2 + StartYPosition) * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
            if (Input.GetKey(KeyCode.W))
                transform.Translate(0, 0, ZMovingMP * koeff);
            if (Input.GetKey(KeyCode.S))
                transform.Translate(0, 0, -ZMovingMP * koeff);
            if (Input.GetKey(KeyCode.A))
                transform.Translate(-XMovingMP * koeff, 0, 0);
            if (Input.GetKey(KeyCode.D))
                transform.Translate(XMovingMP * koeff, 0, 0);
        }
        else if (IsMoving) {
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;
            float MovingDirectionX = MovingVector.x * (x - Screen.width / 2) / 16f * 9f / 1500;
            float MovingDirectionZ = MovingVector.z * (y - Screen.height / 2) / 1500;
            transform.Translate(MovingDirectionX / Sensivity1 * koeff, 0, MovingDirectionZ / Sensivity2 * koeff);
        }
    }

    public void SwitchDirection(string direction) {
        switch (direction) {
            case "Begin":
                MovingVector = new Vector3(1, 0, 1);
                IsMoving = true;
                break;
            case "Finish":
                IsMoving = false;
                MovingVector = new Vector3(0, 0, 0);
                break;
        }
    }
}
