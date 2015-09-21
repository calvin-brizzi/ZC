using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class A_Pathfinding : MonoBehaviour {
    Grid grid;
    PathRequestController request;
    void Awake()
    {
        request = GetComponent<PathRequestController>();
        grid = GetComponent<Grid>();
    }
	//Calculates the path from a start to finish using the A* Pathfinding algorithm
	IEnumerator CalculatePath(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] waypoints = new Vector3[0];
        bool success = false;
        Node startNode = grid.NodeFromPoint(startPosition);//Creates a node from the point
        Node endNode = grid.NodeFromPoint(endPosition);
        if (endNode.Walkable ) { //See if the end node is a walkable object or it is a resource
			Heap<Node> open = new Heap<Node> (grid.MaxGridSize);
			HashSet<Node> closed = new HashSet<Node> ();
			open.Add (startNode);
			while (open.Count > 0) {
				Node current = open.Pop ();//Removes first node in heap and re-sorts it
				closed.Add (current);//Adds the node to the closed list
				if (current == endNode) {
					success = true;
					break;
				}
				foreach (Node neighbour in grid.GetNeighbours(current)) {//Traverses through the neighbours of the nodes
					if (!neighbour.Walkable || closed.Contains (neighbour)) {
						continue;
					}
					int costToNeighbour = current.G + GetDist (current, neighbour);
					if (costToNeighbour < neighbour.G || !open.Contains (neighbour)) {//If the cost to neighbour < neighbouring g value
						neighbour.G = costToNeighbour;
						neighbour.H = GetDist (neighbour, endNode);
						neighbour.Parent = current;
						if (!open.Contains (neighbour)) {
							open.Add (neighbour); //Adds the neighbour to the heap
						}
						else{
							open.UpdateItem(neighbour); // Resort the heap to include the neighbour
						}
					}
				}
			}
		}
        yield return null;//Makes wait for one frame
        if (success) // If successfully calculates path
        {
            waypoints = ReversePath(startNode, endNode); //reverse path since path is stored from end to beginning
        }
        request.FinishedProcessing(waypoints, success);
    }
	//Starts the IEnumerator path so can be processed with a frame delay
    public void StartFindPath(Vector3 beginPath, Vector3 endPath)
    {
        StartCoroutine(CalculatePath(beginPath,endPath));
    }
	// Reverses the calculated path but makes sure to maintain the correct start and end nodes
    Vector3[] ReversePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
		Node current = end;
        while (current != start)
        {
            path.Add(current);
            current = current.Parent;
        }
        Vector3[] points = Simplify(path);
        Array.Reverse(points);
        return points;
    }
	//Removes the duplicates in the path
    Vector3[] Simplify(List<Node> nodes)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 dirOld = Vector2.zero;

        for (int i =1; i <nodes.Count; i++)
        {
            Vector2 dirNew = new Vector2(nodes[i - 1].X - nodes[i].X, nodes[i - 1].Y - nodes[i].Y);
            if (dirNew!=dirOld)//If the old path == the new path its the same point
            {
                waypoints.Add(nodes[i].position);
            }
            dirOld = dirNew;
        }
        return waypoints.ToArray();
    }
	//Calculates the distance from start node to end node
    int GetDist(Node start, Node end)
    {
        int distX = Mathf.Abs(start.X - end.X);
        int distY = Mathf.Abs(start.Y - end.Y);
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }


}
