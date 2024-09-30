using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
using DG.Tweening;
using UnityEngine.Events;

public class AiNav : MonoBehaviour
{
    //1. patrol을 돌 시작 노드와 끝 노드를 결정
    //2. patrol 도중 이동할 다음 노드의 연결이 끊겨있으면 돌아가기
    //3. 도중 플레이어를 만나면 patrol 일시 정지
    
    [Header("플레이어 감지 범위")]
    public float maxDistance = 1f;
    [Header("플레이어")]
    [SerializeField] PlayerController player;

    [Header("시작과 끝 노드")]
    public Node StartNode;
    public Node EndNode;
    public Pathfinder pathfinder;
    
    [SerializeField] private List<Node> possiblePath;
    [SerializeField] private Node nextNode;
    [SerializeField] private Node lastNode;
    [SerializeField] private Node currentNode;
    [SerializeField] private bool isReversing = false;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float moveTime = 1.0f; // 이동 시간 변수

    private Graph graph;
    [SerializeField] private Node hitNode;
    [SerializeField] private bool isMoving = false;
    private Camera mainCamera;
    private bool isPlayerDetected = false;

    public UnityEvent<Node, Node, bool> linkerEvent;



    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        player = FindAnyObjectByType<PlayerController>();
        //pathfinder.Fin
        pathfinder.StartNode = StartNode;
        
        pathfinder.DestinationNode = EndNode;
        //nextNode = StartNode;
        graph = pathfinder.GetComponent<Graph>();
        mainCamera = Camera.main;
        SnapToNearestNode();
        //transform.localRotation = Quaternion.identity;
        
    }

    private void Update()
    {
        DetectPlayer();
        AIPatrol();

    }



    private void AIPatrol()
    {

        //감지하면 Patrol하지 않게
        //추가해야 할 것 패트롤 도중에 이동할 수 없는 길을 만난다면 다시 뒤로 돌도록
        //플레이어 또한 ai가 이동경로 사이에 있으면 이동할 수 없도록 할 것

        //possiblePath = pathfinder.FindBestPathForAI(currentNode, PathManager.instance.newNode, StartNode, EndNode);
        //중간경로 >>> 순환 케이스 


        if (isPlayerDetected)
        {
            return; // 플레이어가 감지되면 이동 중지
        }


        foreach (Edge edge in currentNode.Edges)
        {
            if(lastNode == null)
            {
                Debug.LogWarning("previousNode is null");
            }

            if(edge.neighbor == lastNode)
            {
                if(isReversing)
                {
                    nextNode = lastNode;
                    isReversing = !isReversing;
                }

                continue;
            }
            if (edge.isActive && edge.neighbor.NodeType == Node.NodeState.Flat) //지정 경로 외의 경로 이동 불가
            {
                nextNode = edge.neighbor;

                break;
            }
        }

        if (hitNode != null)
        {
            foreach (Edge edge in hitNode.Edges)
            {
                if (!edge.isActive)
                {
                    edge.isActive = true;
                    break;
                }
            }
        }

        
        
        

        //각 노드 칸마다 현재 플레이어 혹은 ai가 있는지 파악하는 로직 작성 >> 그 다음에 각 플레이어들에게 이웃 노드들에 이전에 만든 조건에 부합하면 해당 방향으로 이동 불가능하도록 할 것 >> 
        //UpdateAnimation("isStop", isStop);
        

        possiblePath = pathfinder.FindPath(currentNode, nextNode, lastNode, StartNode, EndNode);
        
        if (isMoving) return;

        if (possiblePath.Count > 0)
        {
            
            //FollowPath(possiblePath);
            StartCoroutine(FollowPathRoutine(possiblePath));
            lastNode = currentNode;
        }
        else
        {
            isReversing = !isReversing;
        }   
    }

    private void DetectPlayer()
    {
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //저장될 노드들

        
        if (distanceToPlayer <= maxDistance)
        {
            StopAllCoroutines();

            isMoving = false;

            animator.SetBool("isMoving", isMoving);

            // 플레이어가 위치한 노드
            hitNode = player.GetComponentInParent<Node>();

            linkerEvent?.Invoke(currentNode, hitNode, false);

            //ai가 있는 노드와 플레이어가 있는 노드 사이를 비활성화
            // 플레이어가 감지되었을 때 엣지를 비활성화
            
            isPlayerDetected = true;
            return;
        }
        else
        {
            if (isPlayerDetected)
            {
                // 플레이어가 감지되지 않으면 모든 엣지를 다시 활성화
                Debug.Log("Player lost! Re-enabling edges.");

                linkerEvent?.Invoke(currentNode, hitNode, true);

                isPlayerDetected = false;
            }
        }



    }

    private IEnumerator FollowPathRoutine(List<Node> path)
    {
        // start moving
        isMoving = true;

        if (path == null || path.Count <= 1)
        {
            Debug.Log("PLAYERCONTROLLER FollowPathRoutine: invalid path");
        }
        else
        {
            //UpdateAnimation("isMoving", isMoving);
            animator.SetBool("isMoving", isMoving);

            // loop through all Nodes
            // 코루틴 내에서 경로 재탐색 중요************************************
            for (int i = 0; i < path.Count; i++)
            {
                // use the current Node as the next waypoint
                nextNode = path[i];

                // aim at the Node after that to minimize flipping
                int nextAimIndex = Mathf.Clamp(i + 1, 0, path.Count - 1);
                Node aimNode = path[nextAimIndex];
                //FaceNextPosition(transform.position, aimNode.transform.position);
                // move to the next Node
                yield return StartCoroutine(MoveToNodeRoutine(transform.position, nextNode));
            }

        }
        isMoving = false;
        //UpdateAnimation("isMoving", isMoving);
    }

    
    private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
    {
        
        float distance = Vector3.Distance(startPosition, targetNode.transform.position);

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

                if (currentNode.NodeType is Node.NodeState.Ladder || dirBoundary.isTeleport)
                {
                    //Quaternion targetRotation = Quaternion.LookRotation(newDir);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                    FaceNextPosition(transform.position, targetPos);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }


            }


            yield return null;

        }


        //노드에서 노드로 직접 이동?
        if (dirBoundary.isTeleport)
        {
            Boundary revDirBoundary = targetNode.FindEdge(currentNode);


            transform.position = revDirBoundary.transform.position;
        }

        transform.parent = targetNode.transform;
        currentNode.isStacked = false;
        currentNode = targetNode;
        currentNode.isStacked = true;

        targetNode.gameEvent.Invoke();

        startPosition = transform.position;
        elapsedTime = 0;

        while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
        {

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.MoveTowards(startPosition, targetPos, lerpValue);
            Vector3 newDir = targetPos - transform.position;

            if (newDir != Vector3.zero)
            {
                if (currentNode.NodeType is Node.NodeState.Ladder || dirBoundary.isTeleport)
                {

                    FaceNextPosition(transform.position, targetPos);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }
            }

            // wait one frame
            yield return null;
        }
    }


    private void MoveToNode(Vector3 startPosition, Node targetNode)
    {

        float distance = Vector3.Distance(startPosition, targetNode.transform.position);

        float elapsedTime = 0;

        Boundary dirBoundary = currentNode.FindEdge(targetNode);

        if (dirBoundary == null)
        {
            return;
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

                if (currentNode.NodeType is Node.NodeState.Ladder || dirBoundary.isTeleport)
                {
                    //Quaternion targetRotation = Quaternion.LookRotation(newDir);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                    FaceNextPosition(transform.position, targetPos);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }


            }


        }


        //노드에서 노드로 직접 이동?
        if (dirBoundary.isTeleport)
        {
            Boundary revDirBoundary = targetNode.FindEdge(currentNode);


            transform.position = revDirBoundary.transform.position;
        }

        transform.parent = targetNode.transform;
        currentNode.isStacked = false;
        currentNode = targetNode;
        currentNode.isStacked = true;

        targetNode.gameEvent.Invoke();

        startPosition = transform.position;
        elapsedTime = 0;

        while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
        {

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.MoveTowards(startPosition, targetPos, lerpValue);
            Vector3 newDir = targetPos - transform.position;

            if (newDir != Vector3.zero)
            {
                if (currentNode.NodeType is Node.NodeState.Ladder || dirBoundary.isTeleport)
                {

                    FaceNextPosition(transform.position, targetPos);
                }
                else
                {
                    Quaternion targetRotation = Quaternion.LookRotation(newDir, currentNode.transform.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpValue);
                }
            }

        }
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

    public bool HasReachedNode(Node node)
    {
        if (pathfinder == null || graph == null || node == null)
        {
            return false;
        }

        float distanceSqr = (node.transform.position - transform.position).sqrMagnitude;

        return (distanceSqr < 0.01f);
    }
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

    private void UpdateAnimation(string ani_transition, bool isMoving)
    {

        animator.SetBool(ani_transition, isMoving);

        //if (AIAniamtion != null)
        //{
        //    AIAniamtion.ToggleAnimation(isMoving);
        //}
    }
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




}
