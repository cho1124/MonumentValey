using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UnityEngine.UI;

namespace RW.MonumentValley
{
    [Serializable]
    public class TotemSettings
    {
        [Header("���ۼ���")]
        public Transform target;
        public float moveSpeed = 1f;
        public Transform pivot;
        
        
        public int minDragDist = 10;
        public UnityEvent snapEvent; //�̰� ����� ���� �𸣰ڳ�
        public Text DebugText;

        
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;


        //�� �κ��� �θ� ��ü���� �����ؾ� �� �κ�
        private Camera mainCamera;
       
        [SerializeField] public Pathfinder pathfinder;
        private Graph graph;
        [SerializeField] private Node[] Nodes;
        public Node currentNode;

        public Node nextNode;
        public float moveTime = 1f; // �̵� �ð�
        
        public float speed;

        private List<Node> pathNodes = new List<Node>(); // �̵��� ���
        
        private int currentPathIndex = 0;                // ���� ��ο��� �� ��° ���� ������ ����
        private bool isMoving = false;                   // ĳ���Ͱ� �̵� ������ ����
        
        private float elapsedTime = 0f;                  // �̵� �� ��� �ð�
        


        //1. ���� ������Ʈ�ѷ��� �θ�ü�� �޾ƾ� ��
        //2. �׿� ���� pivot���� ü�������� �����ؾ���

        /*�ؾ� �� ��
         1. �� ó���� ���۰� ���� ������ ��带 ã�� ���� ����
         2. �߰��� Node�� isTotemMove�� �߰��Ͽ� isTotemMove ���¿� �ִ� ���� ����� ���
         3. ���콺�� �巡���ؼ� �巡���� ��ġ�� �ִ� ���� ����� ��带 ã�� ��
         4. �÷��̾� ��Ʈ�ѿ� �ִ�  FindBestPath ���� Ȱ���ؼ� �̵���Ű��
         5. ������ Ư�� ��尡 �̵� ������ �Ÿ��� ��� ���� �� Ȱ��ȭ�� �� �ֵ��� Event
         6. ���ۿ� ���� �߰����� ���� ������.....
         7. ��¥ �������� ���������û�̹ٺ�
         */

        public void UpdateDebugText()
        {
            if(DebugText != null)
            {
                DebugText.text = $"currentNodes po : {currentNode.transform.position} \n currentPos : {target.transform.position}";
            }
        }

        public void Initialize()
        {
            mainCamera = Camera.main;

            if (pathfinder != null)
            {
                graph = pathfinder.GetComponent<Graph>();
            }
            
        }

        public List<Node> FindPathStart(Node node)
        {
            return pathfinder.FindBestPathForTotem(node, Nodes);
        }

        public void BeginDrag(Vector2 mousePosition)
        {
            //isSpinning = true;
            //pivot�� �� ���� ����� pivot�� ã��?
            
            
        }

        public void Drag(Vector2 mousePosition)
        {
            if (isActive)
            {
                MoveTotem(mousePosition);
            }
            
        }

        public void EndDrag(Vector2 mousePosition)
        {

        }

        private Node HasNeighbor(Node currentNode, Vector3 mouseDir)
        {
            float maxDot = -1.0f; // ������ �ִ밪�� ���� (���� ������ ������ ã�� ����)
            Edge closestEdge = null;
            Node nextNode = null;
            Vector3 normalizedMouseDir = mouseDir.normalized;

            foreach (Edge e in currentNode.Edges)
            {
                if (e.neighbor != null)
                {
                    
                    // ���� ���� �̿� ��� ������ ���� ���͸� ���ϰ� ����ȭ
                    Vector3 testDir = (e.neighbor.transform.position - currentNode.transform.position).normalized;

                    // �� ������ ������ ���Ͽ� ���絵�� ���
                    float dot = Vector3.Dot(testDir, normalizedMouseDir);


                    if (dot > maxDot && dot > 0.7f) // 0.7f�� ���缺�� �Ǵ��ϴ� �Ӱ谪
                    {
                        maxDot = dot;
                        nextNode = e.neighbor;
                    }


                    //return true;
                }
            }

            return nextNode;
            

            //return false;
        }

        private void MoveTotem(Vector2 mousePosition)
        {
            
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);
            //���콺�� Ÿ�� ������ ���⺤��
            //�׷��� ���� ���� �̿��� ���� ���̿��� ���� ���콺�� Ÿ�� ������ ���⺤�Ϳ� ����� ������ ã�Ƽ� �����Ѵ�?

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            // Ray�� �浹�� ��ü ������ ������ ����
            RaycastHit hit;

            // Ray�� � ��ü�� �浹�ߴ��� Ȯ��
            if (Physics.Raycast(ray, out hit))
            {
                // �浹�� ��ü�� ���� ��ǥ
                Vector3 hitPoint = hit.point;

                // �浹 ��ġ ���
                nextNode = graph?.FindClosestNode(hitPoint, true);
                Debug.Log("Hit Points Node : " + nextNode);

            }
            
            //�Ʒ��κ��� controller �κп��� �����ҵ�
            List<Node> testNodes = pathfinder.FindPath(currentNode, nextNode);

            Debug.Log("in movetototem's testNodes Count : " + testNodes.Count);

            //
            //
            //
            //if (testNodes.Count != 0)
            //{
            //    Debug.Log("nodes Count : " + testNodes.Count);
            //
            //   
            //
            //
            //    foreach(Node node in testNodes)
            //    {
            //       Debug.Log("nodes name : " + node.name);
            //
            //    }
            //    
            //}
            //
            //if (testNodes.Count > 1)
            //{
            //    //StartCoroutine(FollowPathRoutine(newPath));
            //    //StartMoveAlongPath(testNodes);
            //    //�� �κ��� �ڷ�ƾ ���� �ѹ� �غ���
            //}
            //
            //MoveAlongPath(target.transform);
            //
            //
            //Vector3 newDir = (nextNode.transform.position - target.position).normalized;
            //target.position = Vector3.MoveTowards(target.position, nextNode.transform.position, moveSpeed  * 5f * Time.deltaTime);
            //
            //currentNode = nextNode;

            //���⼭ destNode�� ���� ����� �̿� ��尡 �ƴ϶�� ������ �ط������� ���ϰ� ������
            //TODO: �߰����� ��ư�� timeline �����ϸ鼭 ���� ���� �ٽ��ϱ�
            //���� ������ �����ϱ�
            //�� 2�ܰ���� �����ϸ� ������
            //�ܼ��� �̵��ϴ� ���� �ƴ�, directionToMouse�� ���� ����� Node�� ã��
            //�ִ��� �巡�׷δ� �ʹ� �Ҿ����� ���ã��\
            //�ذ����� ��� �����غ��� �׳� ��带 ã�°� �ƴ� �׳� ó������ ������ �̵� ������ ���� ����� �ű⼭�� �̵��� �� �ֵ��� �Ѵ�?
        }

        public void StartMoveAlongPath(List<Node> path)
        {
            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER StartMoveAlongPath: invalid path");
                return;
            }

            pathNodes = path;
            currentPathIndex = 0;
            nextNode = pathNodes[currentPathIndex];
            //isMoving = true;
            elapsedTime = 0f;
            //MoveAlongPath(currentNode.transform);



        }

        // �̵� ������ ó���ϴ� �޼���
        public void MoveAlongPath(Transform currentTr)
        {
            if (nextNode == null) return;

            elapsedTime += Time.deltaTime;
            float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);

            // ���� ��ġ���� ��ǥ ������ Lerp�� �̵�
            Vector3 targetPos = nextNode.transform.position;
            currentTr.position = Vector3.Lerp(currentTr.position, targetPos, lerpValue);

            // ��ǥ ��忡 ������ ���
            if (lerpValue >= 1f)
            {
                // ���� ��� ���� �� ���� ��� ����
                currentTr.parent = nextNode.transform;
                currentNode = nextNode;

                // �ش� ����� �̺�Ʈ ����
                nextNode.gameEvent.Invoke();

                // ���� ���� �̵� �غ�
                currentPathIndex++;
                if (currentPathIndex < pathNodes.Count)
                {
                    nextNode = pathNodes[currentPathIndex];
                    elapsedTime = 0f;
                }
                else
                {
                    // ��� ���� ����
                    isMoving = false;
                    
                }
            }
        }

        public Transform SnapToNearestNode(Transform _transform, bool isSmoothMove)
        {
            Node nearestNode = graph?.FindClosestNode(_transform.position, true);
            
            if(isSmoothMove)
            {
                if (nearestNode != null && nearestNode.canTotemMove)
                {
                    currentNode = nearestNode;
                    
                    _transform.DOMove(currentNode.transform.position, 0.2f);
                    //_transform.position = nearestNode.transform.position;
                }
            }
            else
            {
                if (nearestNode != null && nearestNode.canTotemMove)
                {
                    currentNode = nearestNode;
                    //_transform.DOMove(currentNode.transform.position, 1f);
                    _transform.position = nearestNode.transform.position;
                }
            }
            return _transform;
            
        }

        //���� �̵� �̺�Ʈ���� ����� �� >>> �� �κ��� ���̴ٵ� ����� ���ɼ��� ����.
        //TODO: PlayerController ��ũ��Ʈ������ �߰��ϱ�, �� ��ũ��Ʈ�鿡 ���� ���踦 �ٽ� �ؾ��� ����

        public void SnapToNode(Transform _transform ,Node node)
        {
            currentNode = node;
            _transform.position = node.transform.position;
        }



    }

    public class TotemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Range(0.25f, 2f)]
        [SerializeField] private float moveTime = 0.5f;

       
        [Header("���� ����")]
        [SerializeField] private TotemSettings totemSettings;
        [SerializeField] private Node startNode;
        [SerializeField] private Node destNode;
        [SerializeField] private List<Node> canMoveNodes;
        

        [Header("��Ʈ�� �����")]
        public bool isControlEnabled = false;
        public bool isMoving = false;


        private void Awake()
        {
            totemSettings.Initialize();
            //isMoving = false;
            isControlEnabled = true;
        }

        private void Start()
        {

            totemSettings.SnapToNearestNode(transform, false);
            canMoveNodes = totemSettings.FindPathStart(startNode);
            

            //Vector3.Slerp()
        }

        private void Update()
        {
            totemSettings.UpdateDebugText();
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            
            if (totemSettings.isActive)
            {
                totemSettings.BeginDrag(eventData.position);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            //�巡�׸� ���� �ش��ϴ� ���鸸 �̵��ϱ� >> ��尡 �ִ� ��ġ�� �ƴϾ ���� ������ �̵� ������ ��带 ã�Ƽ� �� �������� ���� �̵��ϴ� �˰���
            if (totemSettings.isActive)
            {
                totemSettings.Drag(eventData.position);

                //Debug.Log("totemSettings nextNode : " + totemSettings.nextNode);

                List<Node> testNodes = totemSettings.pathfinder.FindPath(totemSettings.currentNode, totemSettings.nextNode);

                


                //Debug.Log("currentNode : " + totemSettings.currentNode);

                Debug.Log("testNodes Count : " + testNodes.Count);


                if(testNodes.Count > 1 && Vector3.Distance(transform.position, totemSettings.currentNode.transform.position) < 0.1f)
                {
                    StartCoroutine(FollowPathRoutine(testNodes));
                }
                else
                {
                    isMoving = false;
                }

                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
            //���߿� ������ �ƴ����� �� ���ɼ��� ������ >>> �ƴѰͰ��⵵   
        }
        public IEnumerator FollowPathRoutine(List<Node> path)
        {
            //isMoving = true;

            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER FollowPathRoutine: invalid path");
            }
            else
            {
                

                // loop through all Nodes
                for (int i = 0; i < path.Count; i++)
                {
                    // use the current Node as the next waypoint
                    totemSettings.nextNode = path[i];

                    


                    // move to the next Node
                    yield return StartCoroutine(MoveToNodeRoutine(transform.position, totemSettings.nextNode));
                }
            }

            

        }

        //  lerp to another Node from current position
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
                    totemSettings.currentNode = targetNode;

                    
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
            if (totemSettings.pathfinder == null || node == null)
            {
                return false;
            }

            float distanceSqr = (node.transform.position - transform.position).sqrMagnitude;

            return (distanceSqr < 0.01f);
        }


    }
}


