using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RW.MonumentValley;

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
    private Graph graph;
    public Transform ChildTr;
    [SerializeField] private List<Node> possiblePath;

    private Node nextNode;
    
    [SerializeField] private bool isReversing = false;
    private bool isMoving = false;
    private bool canMove = true;
    private float moveTime = 1.0f; // �̵� �ð� ����
    private Node currentNode;
    private Camera mainCamera;

    private void Start()
    {
        ChildTr.GetComponent<Collider>();
        pathfinder.StartNode = StartNode;
        pathfinder.DestinationNode = EndNode;
        graph = pathfinder.GetComponent<Graph>();
        mainCamera = Camera.main;
        SnapToNearestNode();

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

        if (isMoving) return;

        foreach (Edge edge in currentNode.Edges)
        {
            edge.isActive = true;
        }


        if (!isReversing)
        {
            possiblePath = pathfinder.FindPath(currentNode, EndNode);
            //���� ��ο� �� ��θ� ������ ���� �̵� ������ �ּ��� ��θ� ���ϴ� ��

        }
        else
        {
            possiblePath = pathfinder.FindPath(currentNode, StartNode);
            
        }


        //StartPatrol();
        if (possiblePath.Count > 1)
        {
            StartCoroutine(FollowPathRoutine(possiblePath));
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
            UpdateAnimation();

            // loop through all Nodes
            // �ڷ�ƾ ������ ��� ��Ž�� �߿�************************************
            for (int i = 0; i < path.Count; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, maxDistance))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        foreach (Edge edge in currentNode.Edges)
                        {
                            edge.isActive = false;
                        }
                        //Debug.Log("Player");
                        //StopPatrol �޼��� >>
                        isMoving = false;
                        
                        yield break;
                    }   
                }
                else
                {
                    canMove = true;
                }

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
        UpdateAnimation();

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
                transform.rotation = Quaternion.LookRotation(directionToNextNode);
            }
        }
    }

    private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
    {

        float elapsedTime = 0;

        // validate move time
        moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);

        while (elapsedTime < moveTime && targetNode != null && !HasReachedNode(targetNode))
        {

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            Vector3 targetPos = targetNode.transform.position;
            transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

            // if over halfway, change parent to next node
            if (lerpValue > 0.51f)
            {
                transform.parent = targetNode.transform;
                currentNode = targetNode;

                // invoke UnityEvent associated with next Node
                targetNode.gameEvent.Invoke();
                //Debug.Log("invoked GameEvent from targetNode: " + targetNode.name);
            }

            // wait one frame
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
        Node nearestNode = graph?.FindClosestNode(transform.position);
        if (nearestNode != null)
        {
            currentNode = nearestNode;
            transform.position = nearestNode.transform.position;
        }
    }

    private void UpdateAnimation()
    {
        //if (AIAniamtion != null)
        //{
        //    AIAniamtion.ToggleAnimation(isMoving);
        //}
    }

    private void FindPathPossible()
    {

    }


}
