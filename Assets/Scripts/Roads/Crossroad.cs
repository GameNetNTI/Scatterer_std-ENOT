using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossroad : MonoBehaviour {
    private Field FieldClass;
    private long id = -1;

    private void Awake() {
        FieldClass = Camera.main.GetComponent <Field> ();
    }
    
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0) && id != -1) {
            FieldClass.SetPoint(id);
        }
    }

    public void SetId(long _id) { id = _id; }
}
