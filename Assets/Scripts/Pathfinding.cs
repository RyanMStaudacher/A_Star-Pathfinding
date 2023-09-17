using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Pathfinding : MonoBehaviour
{
    public Grid gridScript;
    public GameObject traversalObject;
    public float moveSpeed = 10f;

    [HideInInspector]
    public bool startCalculations = false;

    private List<Node> openNodes;
    private List<Node> pathNodes;
    private Node startingNode;
    private Node endingNode;
    private Node currentNode;
    private Node traversalTarget;
    private bool hasTraversedPath = false;
    //private bool traversePath = false;
    private bool targetReached = false;
    private int targetIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        gridScript = GetComponent<Grid>();

        openNodes = new List<Node>();
        pathNodes = new List<Node>();

        startingNode = gridScript.startingNode;
        currentNode = startingNode;
        endingNode = gridScript.endingNode;
    }

    // Update is called once per frame
    void Update()
    {
        if(startCalculations)
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
    }

    public void CalculatePath()
    {
        if (currentNode == endingNode)
        {
            targetReached = true;
        }
        else if (currentNode != endingNode)
        {
            currentNode.isOpen = false;

            List<Node> neighbors = gridScript.GetNeighbors(currentNode);

            foreach (Node n in neighbors)
            {
                if (n.walkable && n.isOpen)
                {
                    if (!pathNodes.Contains(n) && !openNodes.Contains(n))
                    {
                        openNodes.Add(n);
                        n.parentNode = currentNode;
                    }
                }
            }

            foreach (Node n in openNodes)
            {
                n.gCost = Vector3.Distance(n.worldPosition, currentNode.worldPosition);
                n.hCost = Vector3.Distance(n.worldPosition, endingNode.worldPosition);
                n.fCost = n.gCost + n.hCost;
            }

            List<Node> openNodesSorted = openNodes.OrderBy(node => node.fCost).ToList();

            currentNode = openNodesSorted.FirstOrDefault();

            openNodes.Remove(currentNode);

            pathNodes.Add(currentNode);
        }
    }

    public void TraversePath()
    {
        if (traversalTarget == null)
        {
            traversalTarget = pathNodes[0];
        }

        if (Vector3.Distance(traversalTarget.worldPosition, traversalObject.transform.position) < 0.1f && traversalTarget != endingNode)
        {
            targetIndex++;
            traversalTarget = pathNodes[targetIndex - 1];
        }
        else if(traversalTarget == endingNode)
        {
            Debug.Log("You did it!\nTotal number of nodes traversed = " + pathNodes.Count);
            hasTraversedPath = true;
        }

        if(!hasTraversedPath)
        {
            traversalObject.transform.position = Vector3.MoveTowards(traversalObject.transform.position, traversalTarget.worldPosition, moveSpeed * Time.deltaTime);
        }
        else if(hasTraversedPath && Vector3.Distance(traversalTarget.worldPosition, traversalObject.transform.position) > 0f)
        {
            traversalObject.transform.position = Vector3.MoveTowards(traversalObject.transform.position, traversalTarget.worldPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void ClickStart()
    {
        startCalculations = true;
    }
}
