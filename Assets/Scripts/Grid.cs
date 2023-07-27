using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //public GameObject startPoint;
    public GameObject startingPoint;
    public GameObject endingPoint;
    public LayerMask unwalkableMask;
    public Vector3 gridWorldSize;
    public float nodeRadius;
    public Node[,,] grid;

    public Node startingNode;
    public Node endingNode;

    float nodeDiameter;
    int gridSizeX, gridSizeY, gridSizeZ;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

        CreateGrid();
        startingNode = NodeFromWorldPosition(startingPoint.transform.position);
        endingNode = NodeFromWorldPosition(endingPoint.transform.position);

        //Debug.Log(startingNode);
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY, gridSizeZ];
        Vector3 gridBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.z / 2) - (Vector3.up * gridWorldSize.y / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 gridPoint = gridBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                    bool walkable = !(Physics.CheckSphere(gridPoint, nodeRadius, unwalkableMask));
                    grid[x, y, z] = new Node(walkable, gridPoint, x, y, z);
                }
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;
        float percentZ = (worldPos.z + gridWorldSize.z / 2) / gridWorldSize.z;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        percentZ = Mathf.Clamp01(percentZ);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

        return grid[x, y, z];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));

        if(grid != null)
        {
            //Node playerNode = NodeFromWorldPosition(startPoint.transform.position);

            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable ? Color.green : Color.red);
                //if (playerNode == n) 
                //{
                //    Gizmos.color = Color.blue; 
                //}
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    if(x == 0 && y == 0 && z == 0)
                    {
                        continue;
                    }

                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    int checkZ = node.gridZ + z;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ)
                    {
                        neighbors.Add(grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbors;
    }
}
