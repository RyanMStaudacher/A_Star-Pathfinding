using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public Grid gridScript;

    //public Transform startingPoint;
    //public Transform endingPoint;
    public GameObject traversalObject;
    public float moveSpeed = 10f;
    public bool traversePath = false;

    private List<Node> openNodes;
    private List<Node> closedNodes;
    public List<Node> pathNodes;

    private Node startingNode;
    private Node endingNode;
    private Node currentNode;
    private Node traversalTarget;

    private bool targetReached = false;
    private int targetIndex = 0;

    private bool hasTraversedPath = false;


    private bool test = false;

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GetComponent<Grid>();

        openNodes = new List<Node>();
        pathNodes = new List<Node>();
        closedNodes = new List<Node>();

        startingNode = gridScript.startingNode;
        openNodes.Add(startingNode);
        startingNode.gCost = 0;
        startingNode.hCost = 0;
        startingNode.fCost = 0;
        currentNode = startingNode;
        endingNode = gridScript.endingNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetReached)
        {
            CalculatePath();
            Debug.Log("Target not reached :(");
        }
        else if (targetReached)
        {
            TraversePath();
            Debug.Log("Target reached!!!!!!!");
        }

        if (traversePath && !hasTraversedPath)
        {
            TraversePath();
        }
    }

    public void CalculatePath()
    {
        //foreach(Node n in openNodes)
        //{
        //    if (n.fCost <= currentNode.fCost)//<- Here is the problem. It compares to the current node, but the current node starting out is zero. It should be comparing to all the rest of the openNodes.
        //    {
        //        currentNode = n;
        //        pathNodes.Add(n);
        //    }
        //}

        IEnumerable<Node> query = from n in openNodes
                                  orderby n.fCost
                                  select n;

        currentNode = query.FirstOrDefault();

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
            //-------^Not sure about all this up here^-----------------

            //------------\/This Works\/-------------------------------
            List<Node> neighbors = gridScript.GetNeighbors(currentNode);
            //-------------^This Works^--------------------------------

            foreach (Node n in neighbors)
            {
                if (n.walkable && n.isOpen)
                {
                    if (!openNodes.Contains(n))
                    {
                        openNodes.Add(n);
                        n.parentNode = currentNode;
                        n.gCost = Vector3.Distance(n.worldPosition, startingNode.worldPosition);
                        n.hCost = Vector3.Distance(n.worldPosition, endingNode.worldPosition);
                        n.fCost = n.gCost + n.hCost;
                    }
                }
            }
        }
    }

    public void TraversePath()
    {
        //foreach(Node n in pathNodes)
        //{
        //    Debug.Log(n.ToString());
        //}

        //if(traversalTarget == null)
        //{
        //    traversalTarget = pathNodes[0];
        //}

        //hasTraversedPath = true;

        //if(Vector3.Distance(traversalTarget.worldPosition, traversalObject.transform.position) < 0.1f)
        //{
        //    targetIndex++;
        //    traversalTarget = pathNodes[targetIndex-1];
        //}

        //traversalObject.transform.position = Vector3.MoveTowards(traversalObject.transform.position, traversalTarget.worldPosition, moveSpeed * Time.deltaTime);

        traversalObject.transform.position = Vector3.MoveTowards(traversalObject.transform.position, currentNode.worldPosition, moveSpeed * Time.deltaTime);
    }
}
