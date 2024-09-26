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
        //pathfinder.Fin
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
        //�߰���� >>> ��ȯ ���̽� 
        
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
        

        //�� ��� ĭ���� ���� �÷��̾� Ȥ�� ai�� �ִ��� �ľ��ϴ� ���� �ۼ� >> �� ������ �� �÷��̾�鿡�� �̿� ���鿡 ������ ���� ���ǿ� �����ϸ� �ش� �������� �̵� �Ұ����ϵ��� �� �� >> 
        //UpdateAnimation("isStop", isStop);
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance)) //������� 1. ����ó�� ����� isactive�� ����, 2. 
        {
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

            Vector3 targetPos = dirBoundary.transform.position;

            
            transform.position = Vector3.MoveTowards(startPosition, targetPos, lerpValue);
            transform.forward = targetPos - startPosition;
            
            
            yield return null;

        }


        //��忡�� ���� ���� �̵�?
        Boundary revDirBoundary = targetNode.FindEdge(currentNode);

        if (dirBoundary.isTeleport)
        {
            transform.position = revDirBoundary.transform.position;
            //currentNode = targetNode;
        }

        
        //Vector3 dir = dirBoundary.transform.position - transform.position;

        //transform.forward = dir;
        //transform.LookAt(dirBoundary.transform);


        if(nextNode != null)
        {
            transform.DORotateQuaternion(nextNode.transform.rotation, 0.3f);
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




        if(dirBoundary.isTeleport)
        {
            yield break;
        }

        while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
        {

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.MoveTowards(startPosition, targetPos, lerpValue);
            transform.forward = targetPos - startPosition;

            // wait one frame
            yield return null;
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

    

}
