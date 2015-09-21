using UnityEngine;
using System.Collections;
using System;
//Optimization for large number of units
public class Heap<T> where T: IHeapItem<T>
{
    T[] items;
    int count;
	//Constructor
    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }
	//Adds item to heap
    public void Add(T item)
    {
        item.Index = count;
        items[count] = item;
        SortUp(item);
        count++;
    }
	//Removes first item from heap
    public T Pop()
    {
        T first = items[0];
        count--;
        items[0] = items[count];
        items[0].Index = 0;
        SortDown(items[0]);
        return first;
    }
	//Reorders the item in the heap
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return count;
        }
    }
	//Checks if item is contained in the heap
    public bool Contains(T item)
    {
        return Equals(items[item.Index],item);
    }
	//Moves an item down the heap
    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item.Index * 2 + 1;
            int childIndexRight = item.Index * 2 + 2;
            int swapIndex = 0;
            if (childIndexLeft<count)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight<count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) //if child index left has a lower priority
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if (item.CompareTo(items[swapIndex])<0)
                {
                    Swap(item,items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

    }
	//Moves a item up the heap
    void SortUp(T item)
    {
        int parentIndex = (item.Index - 1) / 2;
        while (true)
        {
            T parent = items[parentIndex];
            if (item.CompareTo(parent) > 0)
            {
                Swap(item, parent);
            }
            else
            {
                break;
            }
            parentIndex = (item.Index - 1) / 2;
        }
    }
	//Swaps two items in the heap
    void Swap(T item1, T item2)
    {
        items[item1.Index] = item2;
        items[item2.Index] = item1;
        int item1Index = item1.Index;
        item1.Index = item2.Index;
        item2.Index = item1Index;
    }
}
//Interface for Heap item
public interface IHeapItem<T> : IComparable<T>
{
    int Index
    {
        get;
        set;
    }
}


