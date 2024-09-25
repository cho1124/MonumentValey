using UnityEngine;
using System;
using System.Collections.Generic;

namespace RW.MonumentValley
{
    // class to activate/deactivate special Edges between Nodes based on rotation
    [Serializable]
    public class RotationLink
    {
        [Header("Base Rotation이랑 Base Position 다 넣어야 해요. 아마도??")]
        // transform to check
        public Transform linkedTransform;
       
        // euler angle needed to activate link
        
        public Vector3 activeEulerAngle;
        [Header("활성화시켜야 하는 포지션에서의 값")]
        public Vector3 activateWorldPosition;

        [Header("Nodes to activate")]
        
        public Node nodeA;
        public Node nodeB;
    }

    [Serializable]
    public class MoverLink
    {
        
        public Vector3 activeEulerAngle;
        public SpinAxis activeSpinAxis = SpinAxis.X;

        public Transform targetObject;
        public DragSpinner spinnerSetter;

        

        

    }


    
    // activates or deactivates special Edges between Nodes
    public class Linker : MonoBehaviour
    {
        [SerializeField] private RotationLink[] rotationLinks;
        [SerializeField] private MoverLink[] moverLinks;

        // toggle active state of Edge between neighbor Nodes
        public void EnableLink(Node nodeA, Node nodeB, bool state)
        {
            if (nodeA == null || nodeB == null)
                return;

            nodeA.EnableEdge(nodeB, state);
            nodeB.EnableEdge(nodeA, state);
        }

        // enable/disable based on transform's euler angles

        //각 바뀔때 UpdateRotation Links 다시 달기
        public void UpdateRotationLinks()
        {
            Debug.Log("asdsadsad");
            foreach (RotationLink l in rotationLinks)
            {
                
                if (l.linkedTransform == null || l.nodeA == null || l.nodeB == null)
                    continue;

                // check difference between desired and current angle
                Quaternion targetAngle = Quaternion.Euler(l.activeEulerAngle);
                float angleDiff = Quaternion.Angle(l.linkedTransform.rotation, targetAngle);
                Vector3 targetPosition = l.activateWorldPosition;
                
                // enable the linked Edges if the angle matches; otherwise disable
                if (Mathf.Abs(angleDiff) < 0.01f && Vector3.Distance(targetPosition, l.linkedTransform.position) < 0.01f)
                {
                    EnableLink(l.nodeA, l.nodeB, true);
                }
                else
                {
                    EnableLink(l.nodeA, l.nodeB, false);
                }
            }

            foreach(MoverLink d in moverLinks)
            {
                Quaternion targetAngle = Quaternion.Euler(d.activeEulerAngle);
                float angleDiff = Quaternion.Angle(d.targetObject.rotation, targetAngle);

                if (Mathf.Abs(angleDiff) < 0.01f)
                {
                    Debug.Log("asdsad");
                    d.spinnerSetter.settings.spinAxis = d.activeSpinAxis;
                    d.spinnerSetter.settings.CompareAndSwap();
                }
                

            }

        }



        // update links when we begin
        private void Start()
        {
            UpdateRotationLinks();
        }
    }
}