using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PathRequestController : MonoBehaviour {

    Queue<PathRequests> pathRequestQueue = new Queue<PathRequests>();
    PathRequests currentRequest;
    static PathRequestController instance;
    A_Pathfinding a;
    bool isProcessing;
    void Awake()
    {
        instance = this;
        a = GetComponent<A_Pathfinding>();
    }
	//Starts the processing of a path request 
	public static void RequestPath(Vector3 beginPath, Vector3 endPath, Action<Vector3[], bool> callback)
    {
        PathRequests newRequest = new PathRequests(beginPath,endPath,callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.ProcessNext();
    }

    void ProcessNext()
    {
        if (!isProcessing && pathRequestQueue.Count>0)
        {
            currentRequest = pathRequestQueue.Dequeue();
            isProcessing = true;
            a.StartFindPath(currentRequest.beginPath,currentRequest.endPath);
        }
    }
	//Finishes the processing of the path and sets the callback to the path obtained and the sucess of the path request from the A_Pathfinding class
    public void FinishedProcessing(Vector3[] path, bool success)
    {
        currentRequest.callback(path,success);
        isProcessing = false;
        ProcessNext();
    }
	//Struct for storing the path end, path begin, and the callback( an action that returns a vector3 and true if it succeeds)
    struct PathRequests
    {
        public Vector3 beginPath;
        public Vector3 endPath;
        public Action<Vector3[], bool> callback;
		//Constructor
        public PathRequests(Vector3 begin, Vector3 end, Action<Vector3[], bool> callback)
        {
            beginPath = begin;
            endPath = end;
            this.callback = callback;
        }
    }
}
