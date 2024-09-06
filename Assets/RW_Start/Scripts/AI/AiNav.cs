using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
using DG.Tweening;



public class AiNav : MonoBehaviour
{
    //1. patrol을 돌 시작 노드와 끝 노드를 결정
    //2. patrol 도중 이동할 다음 노드의 연결이 끊겨있으면 돌아가기
    //3. 도중 플레이어를 만나면 patrol 일시 정지

    public float maxDistance = 1f;

    

    [Header("시작과 끝 노드")]
    public Node StartNode;
    public Node EndNode;
    public Pathfinder pathfinder;
    public Transform ChildTr;
    private Graph graph;
    [SerializeField] private List<Node> possiblePath;
    [SerializeField] private Node nextNode;
    [SerializeField] private Node lastNode;
    [SerializeField] private Node currentNode;
    private Node hitNode;
    [SerializeField] private bool isReversing = false;

    [SerializeField] private Animator animator;
    public Vector2 testVec = new Vector2(90f, 0f);
    public float speed;

    public bool Circulation = false;


    private bool isMoving = false;
    //private bool canMove = true;
    private float moveTime = 1.0f; // 이동 시간 변수
    private Camera mainCamera;

    private void Start()
    {
        
        ChildTr.GetComponent<Collider>();
        animator = ChildTr.GetComponent<Animator>();
        pathfinder.StartNode = StartNode;
        
        pathfinder.DestinationNode = EndNode;
        nextNode = StartNode;
        graph = pathfinder.GetComponent<Graph>();
        mainCamera = Camera.main;
        SnapToNearestNode();
        //transform.localRotation = Quaternion.identity;
        
    }

    private void Update()
    {
        AIPatrol();
    }

    private void AIPatrol()
    {

        //감지하면 Patrol하지 않게
        //추가해야 할 것 패트롤 도중에 이동할 수 없는 길을 만난다면 다시 뒤로 돌도록
        //플레이어 또한 ai가 이동경로 사이에 있으면 이동할 수 없도록 할 것

        //possiblePath = pathfinder.FindBestPathForAI(currentNode, PathManager.instance.newNode, StartNode, EndNode);

        Rotator();
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
            if (edge.isActive)
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

        RaycastHit hit;
        
        //UpdateAnimation("isStop", isStop);
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance)) //개선방안 1. 이전처럼 노드의 isactive를 제어, 2. 
        {
            //감지 버그 발생, 이거 레이캐스트 하면 안되고, 인접 노드를 탐색한 후 그 노드에 플레이어가 있는지를 개선하는것으로 바꿔야할 것(09.03.TODO) >>> 실제 게임도 정면에서만 감지하는 것이 아닌, 플레이어가 한칸 이내에 있으면 감지됨
            //TODO : 내일 노드 연결된 모듈 테스트 해보기
            //TODO : 진짜진짜 spinner 만들기
            //TODO : 제발좀 놀지 말기, shadergraph를 통해 질감 살리기
            //TODO : 진짜좀 제발좀 인트로 만들기
            //TODO : UI도 제발 만들어야해


            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("player detected");
                isMoving = false;
                animator.SetBool("isMoving", isMoving);
                hitNode = lastNode;
                currentNode.EnableEdge(hitNode, false);

               // currentNode.EnableEdge()
                return;
            }
            
        }

        //foreach (Edge edge in currentNode.Edges) //이거 안돼 저번이랑 똑같은 문제야 해결해야댐.. //위의 ai 멈추는 것에 더불어 플레이어만 경로를 끊든가 해야될듯?
        //{
        //    edge.isActive = true;
        //}

        possiblePath = pathfinder.FindPath(currentNode, nextNode, lastNode, StartNode, EndNode);
        
        if (isMoving) return;

        if (possiblePath.Count > 0)
        {
            StartCoroutine(FollowPathRoutine(possiblePath));
            lastNode = currentNode;
        }
        else
        {
            isReversing = !isReversing;
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
                FaceNextPosition(transform.position, aimNode.transform.position);
                // move to the next Node
                yield return StartCoroutine(MoveToNodeRoutine(transform.position, nextNode));
            }

        }
        isMoving = false;
        //UpdateAnimation("isMoving", isMoving);
    }

    public void FaceNextPosition(Vector3 startPosition, Vector3 nextPosition)
    {
        if (mainCamera == null)
        {
            return;
        }

        // convert next Node world space to screen space
        Vector3 nextPositionScreen = mainCamera.WorldToScreenPoint(nextPosition);

        // convert next Node screen point to Ray
        Ray rayToNextPosition = mainCamera.ScreenPointToRay(nextPositionScreen);

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
                if(!Circulation)
                {
                    transform.rotation = Quaternion.LookRotation(directionToNextNode);

                }
                //transform.forward = directionToNextNode;
            }
        }
    }

    private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
    {

        if(currentNode.isTeleport)
        {
            transform.position = targetNode.transform.position;
            transform.parent = targetNode.transform;
            currentNode = targetNode;
            
            targetNode.gameEvent.Invoke();

            yield break;
        }


        float distance = Vector3.Distance(startPosition, targetNode.transform.position);

        float elapsedTime = 0;

        // validate move time
        moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);

        speed = distance / moveTime;
        float step = speed * Time.deltaTime;
        while (elapsedTime < moveTime && !HasReachedNode(targetNode))
        {
            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(elapsedTime / moveTime);

            // 현재 위치를 목표 위치로 Lerp
            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

            // 이동 중간 지점에서 부모 노드 변경 및 이벤트 호출
            if (lerpValue > 0.51f)
            {
                transform.parent = targetNode.transform;
                currentNode = targetNode;
                targetNode.gameEvent.Invoke();
            }

            // 한 프레임 대기
            yield return null;
        }
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

    private void Rotator()
    {
        //Quaternion newQ = Quaternion.Lerp(currentNode.transform.rotation, nextNode.transform.rotation, Time.deltaTime * 3f);

        if(currentNode.transform.rotation != nextNode.transform.rotation)
        {
            transform.DORotateQuaternion(nextNode.transform.rotation, 0.3f);

            
            //Debug.Log("currentNode rotation : " + currentNode.transform.rotation.eulerAngles);
            //Debug.Log("nextNode rotation : " + nextNode.transform.rotation.eulerAngles);
            //Debug.Log("newQ rotation : " + newQ.eulerAngles);
        }

        //currentNode.transform.rotation;
        //Debug.Log("newQ" + newQ);
        //transform.rotation = newQ;

    }


}
