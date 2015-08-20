using UnityEngine;
using System.Collections;

public class Node : HeapItem<Node>{
    bool walkable;
    public Vector3 position;
	public bool Resource;
    Node parent;
    int g;
    int h;
    int x, y;
    int index;
    public Node(bool walkable,Vector3 position, int x, int y)
    {
        this.walkable = walkable;
        this.position = position;
        this.x = x;
        this.y = y;
    }

	public int CompareTo(Node node)
	{
		int compare = f.CompareTo(node.f);
		if (compare == 0)
		{
			compare = h.CompareTo(node.h);
		}
		return -compare; // Because return the lowest cost node
	}

	#region Getters and Setters
    public int f
    {
        get
        {
            return g + h;
        }
    }

	public Node Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public bool Walkable
	{
		get
		{
			return walkable;
		}
		set
		{
			walkable = value;
		}
	}

    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

	public int G
	{
		get
		{
			return g;
		}
		set
		{
			g = value;
		}
	}
	public int H
	{
		get
		{
			return h;
		}
		set
		{
			h = value;
		}
	}
	public int X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public int Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}
	#endregion
    
}
