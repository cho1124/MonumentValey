﻿
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RW.MonumentValley
{

  
    // handles Player input and movement
    [RequireComponent(typeof(PlayerAnimation))]
    public class PlayerController : MonoBehaviour
    {
        //  time to move one unit
        [Range(0.25f, 2f)]
        [SerializeField] private float moveTime = 0.5f;

        // click indicator
        [SerializeField] Cursor cursor;

        // cursor AnimationController
        private Animator cursorAnimController;

        // layer
        private LayerMask levelLayer;
        private LayerMask playerLayer;
        [SerializeField] private GameObject RenderPlayerObj;


        // pathfinding fields
        //private Clickable[] clickables;
        [SerializeField] private Pathfinder pathfinder;
        private Graph graph;
        private Node currentNode;
        private Node nextNode;

        // flags
        private bool isMoving;
        private bool isControlEnabled;
        private PlayerAnimation playerAnimation;
        

        private void Awake()
        {
            
            playerAnimation = GetComponent<PlayerAnimation>();

            if (pathfinder != null)
            {
                graph = pathfinder.GetComponent<Graph>();
            }

            isMoving = false;
            isControlEnabled = true;
            

        }

        private void Start()
        {
            // always start on a Node
            SnapToNearestNode();

            // automatically set the Graph's StartNode 
            if (pathfinder != null && !pathfinder.SearchOnStart)
            {
                pathfinder.SetStartNode(transform.position);
            }

            if(PathManager.instance.clickables.Length == 0)
            {
                Debug.Log("Shit");
            }

            //listen to all clickEvents
            foreach (Clickable c in PathManager.instance.clickables)
            {
                c.clickAction += OnClick;
            }

            levelLayer = LayerMask.NameToLayer("Level");
            playerLayer = LayerMask.NameToLayer("Player");
        }

        private void OnDisable()
        {
            // unsubscribe from clickEvents when disabled
            foreach (Clickable c in PathManager.instance.clickables)
            {
                c.clickAction -= OnClick;
            }
        }

        private void Update()
        {

            UpdateAnimation();
            CheckInDoor();

           
        }

        private void CheckInDoor()
        {
            
            if(currentNode.isDoor)
            {
                RenderPlayerObj.layer = levelLayer;
                
            }
            else
            {
                RenderPlayerObj.layer = playerLayer;
            }
            
        }

        private void OnClick(Clickable clickable, Vector3 position)
        {
            if (!isControlEnabled || clickable == null || pathfinder == null)
            {
                return;
            }
            

            // find the best path to the any Nodes under the Clickable; gives the user some flexibility
            List<Node> newPath = pathfinder.FindBestPath(currentNode, clickable.ChildNodes);

            // if we are already moving and we click again, stop all previous Animation/motion
            if (isMoving)
            {
                StopAllCoroutines();
            }

            // show a marker for the mouse click
            if (cursor != null)
            {
                cursor.ShowCursor(position);

            }
            else
            {
                Debug.Log("Cursor is null");    
            }

            // if we have a valid path, follow it
            if (newPath.Count > 1)
            {
                StartCoroutine(FollowPathRoutine(newPath));
            }
            else
            {
                // otherwise, invalid path, stop movement
                isMoving = false;

                //여기서 idle상태들 확인
                
            }
        }

        public IEnumerator FollowPathRoutine(List<Node> path)
        {
            // start moving
            isMoving = true;


            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER FollowPathRoutine: invalid path");
            }
            else
            {
                //여기서 이동 상태들 확인
                

                // loop through all Nodes
                for (int i = 0; i < path.Count; i++)
                {
                    // use the current Node as the next waypoint
                    nextNode = path[i];

                    //Debug.Log("nextNode's name : " + nextNode.name);
                    // aim at the Node after that to minimize flipping
                    int nextAimIndex = Mathf.Clamp(i + 1, 0, path.Count - 1);
                    Node aimNode = path[nextAimIndex];
                    //FaceNextPosition(transform.position, aimNode.transform.position);

                    // move to the next Node
                    yield return StartCoroutine(MoveToNodeRoutine(transform.position, nextNode));
                }
            }

            isMoving = false;
            //코루틴 끝나면 다시 idle상태 확인
            

        }

        //  lerp to another Node from current position
        private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
        {

            float elapsedTime = 0;

            Boundary dirBoundary = currentNode.FindEdge(targetNode);

           
            if (dirBoundary == null)
            {
                yield break;   
            }

            // validate move time
            moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);

            
            while (elapsedTime < moveTime && targetNode != null && !HasReachedBoundary(dirBoundary))
            {
                
                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

                Vector3 localTargetPos = dirBoundary.transform.localPosition;  // 타겟의 로컬 좌표
                Vector3 targetPos = dirBoundary.transform.parent.TransformPoint(localTargetPos);


                Vector3 newDir = targetPos - transform.position;

                transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

                if (newDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }

                yield return null;

            }

            if (dirBoundary.isTeleport)
            {
                Boundary revDirBoundary = targetNode.FindEdge(currentNode);


                transform.position = revDirBoundary.transform.position;
            }
            

            transform.parent = targetNode.transform;
            currentNode.isStacked = false;
            currentNode = targetNode;
            currentNode.isStacked = true;

            // invoke UnityEvent associated with next Node
            targetNode.gameEvent.Invoke();
            //Debug.Log("invoked GameEvent from targetNode: " + targetNode.name);

            startPosition = transform.position;
            elapsedTime = 0;

            
            while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
            {

                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

                Vector3 targetPos = targetNode.transform.position;
                transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

                Vector3 newDir = targetPos - transform.position;


                if (newDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }

                // wait one frame
                yield return null;
            }
        }

        // snap the Player to the nearest Node in Game view
        public void SnapToNearestNode()
        {
            Node nearestNode = graph?.FindClosestNode(transform.position, false);
            if (nearestNode != null)
            {
                currentNode = nearestNode;
                currentNode.isStacked = true;
                transform.position = nearestNode.transform.position;
            }
        }

        public void MoveNextStartNode(Node StartNode)
        {
            currentNode.isStacked = false;
            currentNode = StartNode;
            currentNode.isStacked = true;
            transform.position = currentNode.transform.position;
        }


        // turn face the next Node, always projected on a plane at the Player's feet
        private void FaceNextPosition(Vector3 startPosition, Vector3 nextPosition)
        {
            if (Camera.main == null)
            {
                return;
            }

            // convert next Node world space to screen space
            Vector3 nextPositionScreen = Camera.main.WorldToScreenPoint(nextPosition);

            // convert next Node screen point to Ray
            Ray rayToNextPosition = Camera.main.ScreenPointToRay(nextPositionScreen);

            // plane at player's feet
            Plane plane = new Plane(Vector3.up, startPosition);

            // distance from camera (used for projecting point onto plane)
            float cameraDistance = 0f;

            // project the nextNode onto the plane and face toward projected point
            if (plane.Raycast(rayToNextPosition, out cameraDistance))
            {
                Vector3 nextPositionOnPlane = rayToNextPosition.GetPoint(cameraDistance);
                Vector3 directionToNextNode = nextPositionOnPlane - startPosition;
                if (directionToNextNode != Vector3.zero)
                {
                    //transform.rotation = Quaternion.LookRotation(directionToNextNode);
                    transform.forward = directionToNextNode;
                }
            }
        }

        
        // toggle between Idle and Walk animations
        private void UpdateAnimation()
        {
            if(currentNode.NodeType is Node.NodeState.Ladder)
            {
                Debug.Log("currentNode is Ladder");
            }

            
            if (playerAnimation != null)
            {
                if (currentNode.NodeType is Node.NodeState.Ladder || nextNode?.NodeType is Node.NodeState.Ladder)
                {
                    if(nextNode.NodeType is Node.NodeState.Flat)
                    {
                        playerAnimation.StartAnimationParameter("isLadder", false);
                        playerAnimation.StartAnimationParameter("isDown", false);
                    }
                    else
                    {
                        playerAnimation.StartAnimationParameter("isLadder", true);
                    }

                }

                if (currentNode.transform.position.y < nextNode?.transform.position.y)
                {
                    playerAnimation.StartAnimationParameter("isDown", false);


                }
                //upTodown
                else if (currentNode.transform.position.y > nextNode?.transform.position.y)
                {
                    playerAnimation.StartAnimationParameter("isDown", true);
                }
                
                
            }
            playerAnimation.StartAnimationParameter("isMoving", isMoving);
        }

        // have we reached a specific Node?
        public bool HasReachedNode(Node node)
        {
            if (pathfinder == null || graph == null || node == null)
            {
                return false;
            }

            float distanceSqr = (node.transform.position - transform.position).sqrMagnitude;

            return (distanceSqr < 0.01f);
        }

        public bool HasReachedBoundary(Boundary boundary)
        {
            if (pathfinder == null || graph == null || boundary == null)
            {
                return false;
            }

            float distanceSqr = (boundary.transform.position - transform.position).sqrMagnitude;

            return (distanceSqr < 0.01f);
        }

        // have we reached the end of the graph?
        public bool HasReachedGoal()
        {
            if (graph == null)
            {
                return false;
            }
            return HasReachedNode(graph.GoalNode);
        }

        //  enable/disable controls
        public void EnableControls(bool state)
        {
            isControlEnabled = state;
        }
    }
}