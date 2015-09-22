using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
    public Vector2 gridSize;

    public LayerMask obstacle;
    int nodeDiameter;
   	int gridSizeX;
    int gridSizeY;
	int nodeRadius;
	Node[,] grid;


    void Awake()
    {
		nodeRadius = 1;
        nodeDiameter = nodeRadius*2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);
        //CreateGrid();
    }
	//Recreates the grid for when objects are removed and need to update the grid
	public void ReCreateGrid()
	{
		Debug.Log ("ReCreate Grid");
		Invoke ("CreateGrid",0.3f);
	}
	//Creates the grid of a specific size
    public void CreateGrid()
    {
		Debug.Log ("Create Grid");
        grid = new Node[gridSizeX,gridSizeY]; // Array of nodes
        Vector3 bottomLeft = transform.position - Vector3.right*gridSize.x/2 - Vector3.forward*gridSize.y/2;//Bottom left corner since the object is in the middle of the map
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 point = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(point, nodeRadius, obstacle));//Checks for a collision to see whether node is walkable
                grid[x, y] = new Node(walkable,point,x,y);
            }
        }
    }
	//Creates a node from a point in the map
    public Node NodeFromPoint(Vector3 position)
    {
        float percentX = (position.x + gridSize.x/2)/gridSize.x;
        float percentY = (position.z + gridSize.y / 2) / gridSize.y;//Use z since z coordinate corresponds to the y coordinate
		percentX = Mathf.Clamp01(percentX);//Prevents character from being outside of the grid and therefore causing errors
        percentY = Mathf.Clamp01(percentY);//Prevents character from being outside of the grid and therefore causing errors
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX); //So not working with floats ie cant have float number of nodes
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
	//returns all neighbouring nodes
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x<=1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if(x==0 && y == 0)//Not neighbouring node centre node
                {
                    continue;
                }
				if ((node.X + x)>=0 && (node.X + x)<gridSizeX && (node.Y + y) >= 0 && (node.Y + y) < gridSizeY) // If it is a neighbour
                {
					neighbours.Add(grid[(node.X + x),(node.Y + y)]);
                }
            }
        }
        return neighbours;

    }
	//Returns max grid size
    public int MaxGridSize 
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
	//For debuggin purposes so can see grid size
	void OnDrawGizmos(){
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridSize.x, 1, gridSize.y));
		/*
		if(grid!=null){
			foreach(Node n in grid){
				Gizmos.color = (n.Walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.position,Vector3.one *(nodeDiameter-.1f));
			}
		}
		*/
	}
}
