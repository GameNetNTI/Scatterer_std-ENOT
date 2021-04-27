using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class Map : MonoBehaviour {
    [SerializeField]
    private GameObject[] PrefubsRoads;
    [SerializeField]
    private GameObject[] PrefubsCrossroads;
    [SerializeField]
    private GameObject StopSignPrefub;
    [SerializeField]
    private GameObject SpeedSignPrefub;
    [SerializeField]
    private GameObject BuildingPlane;
    [SerializeField]
    private GameObject FieldPrefub;
    private GameObject FieldObject = null;
    private Field FieldClass;

    private void Awake() {
        FieldClass = Camera.main.GetComponent <Field> ();
    }

    private void Start() {
        GenerateMap("sample1");
    }

    private void GenerateRoads(JObject data) {
        JArray dataRoads = (JArray)data["edges"];
        JArray dataCrossroads = (JArray)data["vertexes"];
        Dictionary <long, (int x, int y)> crossroads = new Dictionary <long, (int x, int y)> ();
        Dictionary <long, List <bool>> crossroadsDirection = new Dictionary <long, List <bool>> ();
        
        foreach (var dataCrossroad in dataCrossroads) {
            long id = Convert.ToInt64((string)dataCrossroad["id"]);
            (int x, int y) pos = (x: Convert.ToInt32(dataCrossroad["position"][0]), y: -Convert.ToInt32(dataCrossroad["position"][1]));
            crossroads[id] = pos;
            crossroadsDirection[id] = new List <bool> () {false, false, false, false};
            FieldClass.AddCrossroad(id, pos);
        }
        
        foreach (var dataRoad in dataRoads) {
            long crossroadId1 = Convert.ToInt64((string)dataRoad["from"]);
            long crossroadId2 = Convert.ToInt64((string)dataRoad["to"]);
            int x1 = crossroads[crossroadId1].x;
            int y1 = crossroads[crossroadId1].y;
            int x2 = crossroads[crossroadId2].x;
            int y2 = crossroads[crossroadId2].y;
            float len = (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) - 1;
            FieldClass.AddRoad(crossroadId1, crossroadId2, len);
        }

        List <(long fromId, long toId, bool oriented)> roadList = FieldClass.GetRoadList();

        foreach ((long fromId, long toId, bool oriented) road in roadList) {
            long crossroadId1 = road.fromId;
            long crossroadId2 = road.toId;
            int x1 = crossroads[crossroadId1].x;
            int y1 = crossroads[crossroadId1].y;
            int x2 = crossroads[crossroadId2].x;
            int y2 = crossroads[crossroadId2].y;
            float x = (float)(x1 + x2) / 2;
            float y = (float)(y1 + y2) / 2;
            List <bool> temp = crossroadsDirection[crossroadId1];
            if (x > x1) temp[0] = true;
            else if (y < y1) temp[3] = true;
            else if (x < x1) temp[2] = true;
            else if (y > y1) temp[1] = true;
            crossroadsDirection[crossroadId1] = temp;
            temp = crossroadsDirection[crossroadId2];
            if (x > x2) temp[0] = true;
            else if (y < y2) temp[3] = true;
            else if (x < x2) temp[2] = true;
            else if (y > y2) temp[1] = true;
            crossroadsDirection[crossroadId2] = temp;
            float len = (float)Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) - 1;
            GameObject newRoad = Instantiate(PrefubsRoads[0], new Vector3(x + 0.5f, 0f, y - 0.5f), Quaternion.Euler(0, Mathf.Atan2(y2 - y1, x2 - x1) * 57.3f, 0));
            newRoad.transform.parent = FieldObject.transform;
            newRoad.transform.localScale = new Vector3(len / 10, 0.1f, 0.1f);
            Renderer rend = newRoad.GetComponent <Renderer> ();
            rend.material.mainTextureScale = new Vector2(len / 10, 1);
        }

        foreach (KeyValuePair <long, (int x, int y)> crossroad in crossroads) {
            List <bool> directions = crossroadsDirection[crossroad.Key];
            (int x, int y) pos = crossroad.Value;
            int cnt = 0;
            foreach (bool dir in directions) {
                if (dir) ++cnt;
            }
            GameObject newCrossroad = null;
            if (cnt == 1) {
                if (directions[0]) newCrossroad = Instantiate(PrefubsCrossroads[3], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 90, 0));
                else if (directions[1]) newCrossroad = Instantiate(PrefubsCrossroads[3], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
                else if (directions[2]) newCrossroad = Instantiate(PrefubsCrossroads[3], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 270, 0));
                else if (directions[3]) newCrossroad = Instantiate(PrefubsCrossroads[3], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 180, 0));
            }
            else if (cnt == 2) {
                if (!directions[0] && !directions[1])
                    newCrossroad = Instantiate(PrefubsCrossroads[2], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
                else if (!directions[1] && !directions[2])
                    newCrossroad = Instantiate(PrefubsCrossroads[2], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 270, 0));
                else if (!directions[2] && !directions[3])
                    newCrossroad = Instantiate(PrefubsCrossroads[2], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 180, 0));
                else if (!directions[3] && !directions[0])
                    newCrossroad = Instantiate(PrefubsCrossroads[2], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 90, 0));
                else if (!directions[0] && !directions[2]) newCrossroad = Instantiate(PrefubsRoads[0], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 90, 0));
                else if (!directions[1] && !directions[3]) newCrossroad = Instantiate(PrefubsRoads[0], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
                else newCrossroad = Instantiate(PrefubsCrossroads[0], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
            }
            else if (cnt == 3) {
                if (!directions[0]) newCrossroad = Instantiate(PrefubsCrossroads[1], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 270, 0));
                else if (!directions[1]) newCrossroad = Instantiate(PrefubsCrossroads[1], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
                else if (!directions[2]) newCrossroad = Instantiate(PrefubsCrossroads[1], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 90, 0));
                else if (!directions[3]) newCrossroad = Instantiate(PrefubsCrossroads[1], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 180, 0));
            }
            else if (cnt == 4) newCrossroad = Instantiate(PrefubsCrossroads[0], new Vector3(pos.x + 0.5f, 0f, pos.y - 0.5f), Quaternion.Euler(0, 0, 0));
            else print("Err");
            if (newCrossroad != null) {
                newCrossroad.transform.parent = FieldObject.transform;
                Crossroad CrossroadClass = newCrossroad.GetComponent <Crossroad> ();
                CrossroadClass.SetId(crossroad.Key);
            }
        }
    }

    private void GenerateBuildings(JArray data) {
        foreach (var buildObj in data) {
            (float x, float y) pos = (x: Convert.ToInt32(buildObj["position"][0]), y: Convert.ToInt32(buildObj["position"][1]));
            (float x, float y) size = (x: Convert.ToInt32(buildObj["size"][0]), y: Convert.ToInt32(buildObj["size"][1]));
            GameObject newBuild = Instantiate(BuildingPlane, new Vector3(pos.x + size.x / 2, 0f, -pos.y - size.y / 2), Quaternion.Euler(0, 0, 0));
            newBuild.transform.parent = FieldObject.transform;
            newBuild.transform.localScale = new Vector3(size.x / 10, 1, size.y / 10);
        }
    }

    private void GenerateSigns(JArray data) {
        foreach (var signObj in data) {
            (float x, float y) pos = (x: Convert.ToInt32(signObj["position"][0]), y: Convert.ToInt32(signObj["position"][1]));
            GameObject newSign = null;
            if ((string)signObj["type"] == "STOP") newSign = Instantiate(StopSignPrefub, new Vector3(pos.x, 0f, -pos.y), Quaternion.Euler(0, 0, 0));
            if ((string)signObj["type"] == "SPEED_LIMIT") newSign = Instantiate(SpeedSignPrefub, new Vector3(pos.x, 0f, -pos.y), Quaternion.Euler(0, 0, 0));
            if (newSign != null) newSign.transform.parent = FieldObject.transform;
        }
    }

    public void GenerateMap(string fileName) {
        if (FieldObject != null) Destroy(FieldObject);
        FieldObject = Instantiate(FieldPrefub, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        
        JObject dataMap = FileReader.LoadJSON(fileName);
        if (dataMap["error"] == null) {
            FieldClass.CreateNewField();
            GenerateRoads((JObject)dataMap["map"]["road"]);
            GenerateBuildings((JArray)dataMap["map"]["buildings"]);
            GenerateSigns((JArray)dataMap["map"]["signs"]);
        }
        else Debug.LogError(dataMap["error"]);
    }
}
