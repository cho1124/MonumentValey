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
        
        
        [Header("활성화시켜야 하는 트랜스폼 값")]
        public Transform activeTr;

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
            
            foreach (RotationLink l in rotationLinks)
            {
                
                if (l.linkedTransform == null || l.nodeA == null || l.nodeB == null)
                    continue;

                // check difference between desired and current angle

                if (l.activeTr == null) continue;

                Quaternion targetAngle = l.activeTr.localRotation;
                float angleDiff = Quaternion.Angle(l.linkedTransform.localRotation, targetAngle);
                Vector3 targetPosition = l.activeTr.localPosition;



                if (Mathf.Abs(angleDiff) < 0.05f && Vector3.Distance(targetPosition, l.linkedTransform.localPosition) < 0.05f)
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
                    
                    d.spinnerSetter.settings.spinAxis = d.activeSpinAxis;
                    d.spinnerSetter.settings.CompareAndSwap();
                }

            }

        }



        // update links when we begin
        private void Start()
        {
            Debug.Log("StartedObj : " + transform.name);
            UpdateRotationLinks();
        }
    }
}