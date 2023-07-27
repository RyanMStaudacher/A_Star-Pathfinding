using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Grid gridScript;

    //public Transform startingPoint;
    //public Transform endingPoint;
    public GameObject traversalObject;
    public float moveSpeed = 10f;

    private List<Node> openNodes;
    private List<Node> closedNodes;
    private List<Node> pathNodes;

    private Node startingNode;
    private Node endingNode;
    private Node currentNode;
    private Node traversalTarget;

    private bool targetReached = false;
    private int targetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GetComponent<Grid>();

        openNodes = new List<Node>();
        closedNodes = new List<Node>();

        startingNode = gridScript.startingNode;
        endingNode = gridScript.endingNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetReached)
        {
            CalculatePath();
        }
        else if (targetReached)
        {
            TraversePath();
        }
    }

    public void CalculatePath()
    {
        foreach(Node n in openNodes)
        {
            if (n.fCost < currentNode.fCost)
            {
                currentNode = n;
                pathNodes.Add(currentNode);
            }
        }

        currentNode.isOpen = false;
        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);

        if(currentNode == endingNode)
        {
            targetReached = true;
            //pathNodes.Add(endingNode);
        }
        else if(currentNode != endingNode) 
        {
            List<Node> neighbors = gridScript.GetNeighbors(currentNode);

            foreach (Node n in neighbors)
            {
                if (n.walkable && n.isOpen)
                {
                    if (!openNodes.Contains(n))
                    {
                        openNodes.Add(n);
                        n.parentNode = currentNode;
                        //pathNodes.Add(currentNode);
                        n.gCost = Vector3.Distance(n.worldPosition, startingNode.worldPosition);
                        n.hCost = Vector3.Distance(n.worldPosition, endingNode.worldPosition);
                        n.fCost = n.gCost + n.hCost;
                    }
                    //else if (openNodes.Contains(n))
                    //{

                    //}
                }
            }
        }
    }

    public void TraversePath()
    {
        if(traversalTarget == null)
        {
            traversalTarget = pathNodes[0];
        }

        if(Vector3.Distance(traversalTarget.worldPosition, traversalObject.transform.position) < 0.1f)
        {
            targetIndex++;
            traversalTarget = pathNodes[targetIndex];
        }

        traversalObject.transform.position = Vector3.MoveTowards(traversalObject.transform.position, traversalTarget.worldPosition, moveSpeed * Time.deltaTime);
    }
}
