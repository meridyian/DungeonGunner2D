
using System;
using UnityEngine;


// To be able to sort the nodes we need to use interface: IComparable
// Sort instances
// pick the node with lowest fcost


public class Node : IComparable<Node>
{

    public Vector2Int gridPosition; 
    public int gCost = 0;               // distance from starting node
    public int hCost = 0;           // distance from finishing node
    public Node parentNode;

    
    // node constructor

    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        
        parentNode = null;
    }



    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    // how your class instances are sorted

    public int CompareTo(Node nodeToCompare)
    {
        // compare will be <0 if this instance Fcost is less than nodeToCompare.FCost
        // compare will be >0 if this instance Fcost is greater than no deToCompare.FCost
        // compare will be ==0 if the values are the same

        int compare = FCost.CompareTo(nodeToCompare.FCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return compare;

    }
}
