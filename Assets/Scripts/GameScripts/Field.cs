using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField]
    private LineRenderer PathLine;

    private Dictionary <long, List <(long toId, float dist)>> graph;
    private List <(long fromId, long toId, bool oriented)> roadList;
    private Dictionary <long, (int x, int y)> crossroadList;

    [SerializeField]
    private long pointFrom = -1, pointTo = -1;

    private (Dictionary <long, long> parent, Dictionary <long, float> dist) Dijkstra(long start) {
        Dictionary <long, float> dist = new Dictionary <long, float> ();
        Dictionary <long, long> parent = new Dictionary <long, long> ();
        Dictionary <long, bool> used = new Dictionary <long, bool> ();
        foreach (KeyValuePair <long, List <(long toId, float dist)>> vertex in graph) {
            dist[vertex.Key] = (float)(1e9 + 7);
            parent[vertex.Key] = -1;
            used[vertex.Key] = false;
        }
        SortedSet <(float w, long v)> queueEdge = new SortedSet <(float w, long v)> ();
        dist[start] = 0;
        queueEdge.Add((0, start));
        while (queueEdge.Count > 0) {
            (float w, long v) p = queueEdge.Min;
            queueEdge.Remove(p);
            used[p.v] = true;
            for (int i = 0; i < graph[p.v].Count; ++i) {
                long u = graph[p.v][i].toId;
                float w = graph[p.v][i].dist;
                if (!used[u] && dist[u] > p.w + w) {
                    queueEdge.Remove((dist[u], u));
                    dist[u] = p.w + w;
                    parent[u] = p.v;
                    queueEdge.Add((dist[u], u));
                }
            }
        }
        return (parent, dist);
    }

    private void CalcPath() {
        (Dictionary <long, long> parent, Dictionary <long, float> dist) graphData = Dijkstra(pointFrom);
        List <long> pathId = new List <long> ();
        for (long id = pointTo; id != pointFrom; id = graphData.parent[id]) {
            pathId.Add(id);
        }
        pathId.Add(pointFrom);
        pathId.Reverse();
        PathLine.positionCount = pathId.Count;
        for (int i = 0; i < pathId.Count; ++i) {
            (int x, int y) pos = crossroadList[pathId[i]];
            PathLine.SetPosition(i, new Vector3(pos.x + 0.5f, 0.5f, pos.y - 0.5f));
        }
        pointFrom = pointTo = -1;
    }

    public List <(long fromId, long toId, bool oriented)> GetRoadList() {
        return roadList;
    }

    public void CreateNewField() {
        roadList = new List <(long fromId, long toId, bool oriented)> ();
        crossroadList = new Dictionary <long, (int x, int y)> ();
        graph = new Dictionary <long, List <(long toId, float dist)>> ();
        PathLine.positionCount = 0;
    }

    public void AddCrossroad(long id, (int x, int y) pos) {
        graph[id] = new List <(long toId, float dist)> ();
        crossroadList[id] = pos;
    }

    public void AddRoad(long fromId, long toId, float len) {
        bool p = true;
        for (int i = 0; i < roadList.Count; ++i) {
            if (roadList[i].fromId == toId && roadList[i].toId == fromId) {
                (long fromId, long toId, bool oriented) tempData = roadList[i];
                tempData.oriented = false;
                roadList[i] = tempData;
                p = false;
            }
        }
        if (p) roadList.Add((fromId: fromId, toId: toId, oriented: true));
        graph[fromId].Add((toId: toId, dist: len));
    }

    public void SetPoint(long id) {
        if (pointFrom == -1) pointFrom = id;
        else {
            pointTo = id;
            CalcPath();
        }
    }
}
