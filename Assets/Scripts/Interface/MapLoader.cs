using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour {
    [SerializeField]
    private Text mapName;
    private Map MapClass;

    private void Awake() {
        MapClass = Camera.main.GetComponent <Map> ();
    }

    public void LoadMap() {
        if (mapName.text == "") Debug.LogError("mapName is empty!");
        else MapClass.GenerateMap(mapName.text);
    }
}
