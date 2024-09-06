using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;
using DG.Tweening;



public class AiNav : MonoBehaviour
{
    //1. patrol�� �� ���� ���� �� ��带 ����
    //2. patrol ���� �̵��� ���� ����� ������ ���������� ���ư���
    //3. ���� �÷��̾ ������ patrol �Ͻ� ����

    public float maxDistance = 1f;

    

    [Header("���۰� �� ���")]
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
    private float moveTime = 1.0f; // �̵� �ð� ����
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

        //�����ϸ� Patrol���� �ʰ�
        //�߰��ؾ� �� �� ��Ʈ�� ���߿� �̵��� �� ���� ���� �����ٸ� �ٽ� �ڷ� ������
        //�÷��̾� ���� ai�� �̵���� ���̿� ������ �̵��� �� ������ �� ��

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
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance)) //������� 1. ����ó�� ����� isactive�� ����, 2. 
        {
            //���� ���� �߻�, �̰� ����ĳ��Ʈ �ϸ� �ȵǰ�, ���� ��带 Ž���� �� �� ��忡 �÷��̾ �ִ����� �����ϴ°����� �ٲ���� ��(09.03.TODO) >>> ���� ���ӵ� ���鿡���� �����ϴ� ���� �ƴ�, �÷��̾ ��ĭ �̳��� ������ ������
            //TODO : ���� ��� ����� ��� �׽�Ʈ �غ���
            //TODO : ��¥��¥ spinner �����
            //TODO : ������ ���� ����, shadergraph�� ���� ���� �츮��
            //TODO : ��¥�� ������ ��Ʈ�� �����
            //TODO : UI�� ���� ��������


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

        //foreach (Edge edge in currentNode.Edges) //�̰� �ȵ� �����̶� �Ȱ��� ������ �ذ��ؾߴ�.. //���� ai ���ߴ� �Ϳ� ���Ҿ� �÷��̾ ��θ� ���簡 �ؾߵɵ�?
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
            // �ڷ�ƾ ������ ��� ��Ž�� �߿�************************************
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

            // ���� ��ġ�� ��ǥ ��ġ�� Lerp
            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

            // �̵� �߰� �������� �θ� ��� ���� �� �̺�Ʈ ȣ��
            if (lerpValue > 0.51f)
            {
                transform.parent = targetNode.transform;
                currentNode = targetNode;
                targetNode.gameEvent.Invoke();
            }

            // �� ������ ���
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
