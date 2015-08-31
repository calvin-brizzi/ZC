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

	IEnumerator CalculatePath(Vector3 startPosition, Vector3 endPosition)
    {
        Vector3[] waypoints = new Vector3[0];
        bool success = false;
        Node startNode = grid.NodeFromPoint(startPosition);
        Node endNode = grid.NodeFromPoint(endPosition);
        if (endNode.Walkable ) { //See if the end node is a walkable object or it is a resource
			Heap<Node> open = new Heap<Node> (grid.MaxGridSize);
			HashSet<Node> closed = new HashSet<Node> ();
			open.Add (startNode);
			while (open.Count > 0) {
				Node current = open.Pop ();//Removes first node in heap and re-sorts it
				closed.Add (current);
				if (current == endNode) {
					success = true;
					break;
				}
				foreach (Node neighbour in grid.GetNeighbours(current)) {
					if (!neighbour.Walkable || closed.Contains (neighbour)) {
						continue;
					}
					int costToNeighbour = current.G + GetDist (current, neighbour);
					if (costToNeighbour < neighbour.G || !open.Contains (neighbour)) {
						neighbour.G = costToNeighbour;
						neighbour.H = GetDist (neighbour, endNode);
						neighbour.Parent = current;
						if (!open.Contains (neighbour)) {
							open.Add (neighbour);
						}
						else{
							open.UpdateItem(neighbour);
						}
					}
				}
			}
		}
        yield return null;//Makes wait for one frame
        if (success)
        {
            waypoints = ReversePath(startNode, endNode);
        }
        request.FinishedProcessing(waypoints, success);
    }

    public void StartFindPath(Vector3 beginPath, Vector3 endPath)
    {
        StartCoroutine(CalculatePath(beginPath,endPath));
    }

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

    Vector3[] Simplify(List<Node> nodes)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 dirOld = Vector2.zero;

        for (int i =1; i <nodes.Count; i++)
        {
            Vector2 dirNew = new Vector2(nodes[i - 1].X - nodes[i].X, nodes[i - 1].Y - nodes[i].Y);
            if (dirNew!=dirOld)
            {
                waypoints.Add(nodes[i].position);
            }
            dirOld = dirNew;
        }
        return waypoints.ToArray();
    }

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
