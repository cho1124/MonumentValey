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

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace RW.MonumentValley
{

    // allows a target Transform to be rotated based on mouse click and drag
    [RequireComponent(typeof(Collider))]
    public class DragSpinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public enum SpinAxis
        {
            X,
            Y,
            Z
        }

        // transform to spin
        [SerializeField] private Transform targetToSpin;

        // axis of rotation
        [SerializeField] private SpinAxis spinAxis = SpinAxis.X;

        // used to calculate angle to mouse pointer
        [SerializeField] private Transform pivot;

        // minimum distance in pixels before activating mouse drag
        [SerializeField] private int minDragDist = 10;

        //[SerializeField] private Linker linker;

        // vector from pivot to mouse pointer
        private Vector2 directionToMouse;

        // are we currently spinning?
        private bool isSpinning;

        private bool isActive;

        // angle (degrees) from clicked screen position 
        private float angleToMouse;

        // angle to mouse on previous frame
        private float previousAngleToMouse;

        // Vector representing axis of rotation
        private Vector3 axisDirection;

        public UnityEvent snapEvent;

        private float timeCount;

        [Header("이동할 수 있는 앵글")]
        [SerializeField] private List<float> allowedAngle;
        [SerializeField] private bool canMoveState = false;
        [SerializeField] private List<Node> edgeNodes;


        void Start()
        {
            switch (spinAxis)
            {
                case (SpinAxis.X):
                    axisDirection = Vector3.right;
                    break;
                case (SpinAxis.Y):
                    axisDirection = Vector3.up;
                    break;
                case (SpinAxis.Z):
                    axisDirection = Vector3.forward;
                    break;
            }
            EnableSpinner(true);
            //CanMoveNode();
        }

        // begin spin drag
        public void OnBeginDrag(PointerEventData data)
        {
            if (!isActive)
            {
                return;
            }

            isSpinning = true;

            // get the angle to the mouse position on down frame
            Vector3 inputPosition = new Vector3(data.position.x, data.position.y, 0f);
            directionToMouse = inputPosition - Camera.main.WorldToScreenPoint(pivot.position);

            // store the angle to mouse pointer on down frame
            previousAngleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        }

        // end spin on mouse release; then round to right angle
        public void OnEndDrag(PointerEventData data)
        {
            if (isActive)
            {
                SnapSpinner();
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (isSpinning && Camera.main != null && pivot != null && isActive)
            {
                // get the angle to the current mouse position
                Vector3 inputPosition = new Vector3(data.position.x, data.position.y, 0f);
                directionToMouse = inputPosition - Camera.main.WorldToScreenPoint(pivot.position);
                angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

                // if we have dragged a minimum threshold, rotate the target to follow the mouse movements around the pivot
                // (left-handed coordinate system; positive rotations are clockwise)
                if (directionToMouse.magnitude > minDragDist)
                {
                    Vector3 newRotationVector = (previousAngleToMouse - angleToMouse) * axisDirection;
                    targetToSpin.Rotate(newRotationVector);
                    previousAngleToMouse = angleToMouse;
                }
            }
        }

        // release and snap to 90-degrees interval
        private void SnapSpinner()
        {
            isSpinning = false;

            // snap to nearest 90-degree interval
            RoundToRightAngles(targetToSpin);

            // invoke event (e.g. to update the SpinnerControl)
            if (snapEvent != null)
            {
                
                snapEvent.Invoke();
            }
        }

        // round to nearest 90 degrees
        private void RoundToRightAngles(Transform xform)
        {
            float roundedXAngle = Mathf.Round(xform.eulerAngles.x / 90f) * 90f;
            float roundedYAngle = Mathf.Round(xform.eulerAngles.y / 90f) * 90f;
            float roundedZAngle = Mathf.Round(xform.eulerAngles.z / 90f) * 90f;

            xform.eulerAngles = new Vector3(roundedXAngle, roundedYAngle, roundedZAngle);
        }

        //enable/disable
        public void EnableSpinner(bool state)
        {
            isActive = state;

            // force snap the spinner on disable
            if (!isActive)
            {
                SnapSpinner();
            }
        }


        //개선점 1. 앞으로 ai추가 할 예정인데 이런 방식으로는 아예 ai 사용이 불가능할 것 따라서 이 부분을 끝 점으로 개선해야 한다
        //이에 대한 해결방안 1. 그냥 인스펙터로 끝점 받아서 처리하기
        //2. 어떻게든 스크립트 내부에서 받아오기
        //1번안 차용, 기존의 더 좋은 스크립트 차용
        //public void CanMoveNode()
        //{
        //    if(allowedAngle.Count <= 0)
        //    {
        //        Debug.LogError("할당 안했어 바보야");
        //        return;
        //    }
        //
        //    Node[] nodes = targetToSpin.gameObject.GetComponentsInChildren<Node>();
        //
        //    if (nodes.Length == 0)
        //    {
        //        Debug.LogError("여기도 할당 안했어 바보야");
        //    }
        //
        //    for (int i = 0; i < allowedAngle.Count; i++)
        //    {
        //        if(targetToSpin.rotation.eulerAngles.x == allowedAngle[i])
        //        {
        //            foreach(Node node in edgeNodes)
        //            {
        //                if(node.Edges.Count == 0)
        //                {
        //                    Debug.LogError("이건 이웃 엣지가 없는 무언가 이름은 : " + node.name);
        //                    continue;
        //                }
        //                
        //                foreach(Edge edge in node.Edges)
        //                {
        //
        //                    foreach(Edge newNode in edge.neighbor.Edges)
        //                    {
        //                        if(newNode.neighbor == node)
        //                        {
        //                            newNode.isActive = true;
        //                            edge.isActive = true;
        //                            break;
        //                        }
        //                    }
        //
        //                }
        //            }
        //
        //            //return; //멍청이
        //        }
        //        else
        //        {
        //            foreach (Node node in edgeNodes)
        //            {
        //                if (node.Edges.Count == 0)
        //                {
        //                    Debug.LogError("이건 이웃 엣지가 없는 무언가 입니다 이름은 : " + node.name);
        //                    continue;
        //                }
        //
        //                foreach (Edge edge in node.Edges)
        //                {
        //
        //                    foreach (Edge newNode in edge.neighbor.Edges)
        //                    {
        //                        //Debug.Log($"neightbor노드의 루트 이름 : {newNode.neighbor.transform.root.name}, 해당 노드의 루트 이름 : {node.transform.root.name}");
        //
        //                        if (newNode.neighbor == node && node.transform.root.name != edge.neighbor.transform.root.name)
        //                        {
        //                            //가정 1. bridge 오브젝트의 node 와 다른 오브젝트의 node의 부모 transform이 다르단걸 이용해서 이동할 수 있는 범위를 재계산할 수 있도록
        //                            //루트 이름을 받아오는 원시적인 방식
        //                            newNode.isActive = false;
        //                            edge.isActive = false;
        //                            break;
        //                        }
        //                    }
        //
        //                }
        //            }
        //        }
        //    }
        //
        //
        //}
    }
}