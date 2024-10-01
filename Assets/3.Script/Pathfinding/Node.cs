
/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RW.MonumentValley
{
    public class Node : MonoBehaviour
    {
        public enum NodeState
        {
            Flat,
            Stair,
            Ladder
        }

        [Header("노드 타입")]
        [SerializeField] private NodeState nodeState = NodeState.Flat;
        public NodeState NodeType => nodeState;

        // gizmo colors
        [SerializeField] private float gizmoRadius = 0.1f;
        [SerializeField] private Color defaultGizmoColor = Color.black;
        [SerializeField] private Color selectedGizmoColor = Color.blue;
        [SerializeField] private Color inactiveGizmoColor = Color.gray;
        

        // neighboring nodes + active state
        [SerializeField] private List<Edge> edges = new List<Edge>();

        // Nodes specifically excluded from Edges
        [SerializeField] private List<Node> excludedNodes;

        //public bool isTeleport = false;

        // reference to the graph
        private Graph graph;

        // previous Node that forms a "breadcrumb" trail back to the start
        private Node previousNode;

        // invoked when Player enters this node
        public UnityEvent gameEvent;

        //public NodeState currentNodeState = NodeState.Up;

        // properties
        // totem Move Node
        [Header("토템 이동 가능 여부")]
        public bool canTotemMove = false;

        [Header("어떤 오브젝트건 위에 있는지 여부")]
        public bool isStacked = false;
        [Header("플레이어가 위에 있는지")]
        public bool isPlayerStacked = false;
        [Header("문 안 트릭")]
        public bool isDoor = false;
        
        public bool canAccessPlayer = true;
        public bool canAccessAI = true;
        public bool canAccessTotem = true;

        public Node PreviousNode { get { return previousNode; } set { previousNode = value; } }

        public Boundary[] boundaries;
        public List<Edge> Edges => edges;

        // 3d compass directions to check for horizontal neighbors automatically(east/west/north/south)
        public static Vector3[] neighborDirections =
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, -1f),
            
        };

        private void Start()
        {
            
            // automatic connect Edges with horizontal Nodes
            if (graph != null)
            {
                FindNeighbors();
            }

            boundaries = GetComponentsInChildren<Boundary>();

        }

        // draws a sphere gizmo
        private void OnDrawGizmos()
        {
            Gizmos.color = defaultGizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);

            Gizmos.color = Color.blue;
            foreach(Boundary boundary in boundaries)
            {
                Gizmos.DrawCube(boundary.transform.position, Vector3.one * 0.1f);
            }

            

            foreach(Edge edge in edges)
            {
                
                Boundary revDirBoundary = edge.neighbor.FindEdge(this);
                if (edge.connectionBoundary != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawCube(edge.connectionBoundary.transform.position, Vector3.one * 0.1f);
                    if(revDirBoundary != null)
                    {
                        Gizmos.DrawCube(revDirBoundary.transform.position, Vector3.one * 0.1f);

                    }
                }

                if (edge.neighbor != null)
                {

                    Gizmos.color = (edge.isActive) ? selectedGizmoColor : inactiveGizmoColor;
                    if(revDirBoundary != null)
                    {

                        Gizmos.DrawLine(revDirBoundary.transform.position, edge.connectionBoundary.transform.position);
                    }
                }
            }
        }

        // draws a sphere gizmo in a different color when selected
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = selectedGizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);

        }

        // fill out edge connections to neighboring nodes automatically
        public void FindNeighbors()
        {
            foreach(Boundary boundary in boundaries)
            {
                Node newNode = graph?.FindNodeAtBoundary(boundary.transform.position, this);
                if(newNode != null)
                {
                    //Debug.Log("currentNode's name : " + transform.name + " newNode's name : " + newNode.name);
                }
                if (newNode != null && !HasNeighbor(newNode) && !excludedNodes.Contains(newNode))
                {
                    Edge newEdge = new Edge { neighbor = newNode, isActive = true, connectionBoundary = boundary};
                    edges.Add(newEdge);
                }

            }

            //foreach (Vector3 direction in neighborDirections)
            //{
            //    //이 부분 바꾸기
            //    Node newNode = graph?.FindNodeAt(transform.position + direction);
            //    //Debug.Log("newNode's Name : " + newNode.name);
            //
            //    
            //
            //    // add to edges list if not already included and not excluded specifically
            //    if (newNode != null && !HasNeighbor(newNode) && !excludedNodes.Contains(newNode))
            //    {
            //
            //        Edge newEdge = new Edge { neighbor = newNode, isActive = true};
            //        edges.Add(newEdge);
            //    }
            //}

            //현재 노드와 엣지에서의 이웃 노드들의 바운더리를 각각 구해서 그 값이 0과 유사할 때 특정한 무언가에 넣고 이동할 때 먼저 그 무언가로 이동한 뒤에 넘어가도록 할 것

            
        }

        // is a Node already in the Edges List?
        private bool HasNeighbor(Node node)
        {
            foreach (Edge e in edges)
            {
                if (e.neighbor != null && e.neighbor.Equals(node))
                {
                    return true;
                }
            }
            return false;
        }

        // given a specific neighbor, sets active state
        public void EnableEdge(Node neighborNode, bool state)
        {
            foreach (Edge e in edges)
            {
                if (e.neighbor.Equals(neighborNode))
                {
                    e.isActive = state;
                }
            }
        }

        public Boundary FindEdge(Node nextNode)
        {
            foreach(Edge e in edges)
            {
                if(e.neighbor.Equals(nextNode))
                {
                    return e.connectionBoundary;
                }
            }
            return null;
        }

        

        public void InitGraph(Graph graphToInit)
        {
            this.graph = graphToInit;
        }
    }
}