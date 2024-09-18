using UnityEngine;

// connection/link to neighboring node
namespace RW.MonumentValley
{
    [System.Serializable]
    public class Edge
    {
        // Tutorial Start3
        public Node neighbor;
        public bool isActive;
        public Vector3 tr;
        public Boundary connectionBoundary;
        //public Transform Tr => tr;
    }

    

    
}