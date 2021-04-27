using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class FileReader : MonoBehaviour {
    private void Start() {
        // Debug.Log("Streaming Assets Path: " + Application.streamingAssetsPath);
    }

    public static JObject LoadJSON(string fileName) {
        string path = Path.Combine(Application.streamingAssetsPath, fileName) + ".json";

        if (File.Exists(path)) return JObject.Parse((string)(File.ReadAllText(path)));
        else return JObject.Parse("{ \"error\": \"File (" + fileName + ".json) has not found\" }");
    }
}
