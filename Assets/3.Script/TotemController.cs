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
        public Transform pivot;
        public float moveSpeed = 1f;
        public int minDragDist = 10;
        public UnityEvent snapEvent; //�̰� ����� ���� �𸣰ڳ�
        
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;

        //�� �κ��� �θ� ��ü���� �����ؾ� �� �κ�
        private Camera mainCamera;
       
        [Header("�̵� ������ ��忡 ���� �κ�")]
        [SerializeField] private Node[] Nodes;

        public Graph graph;
        public Pathfinder pathfinder;
        public Node currentNode;
        public Node lastNode;
        public Node nextNode;
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
            //pivot�� �� ���� ����� pivot�� ã��
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


        private void MoveTotem(Vector2 mousePosition)
        {
            
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
                    currentNode.isStacked = true;
                    
                    _transform.DOMove(currentNode.transform.position, 0.2f);
                    //_transform.position = nearestNode.transform.position;
                }
            }
            else
            {
                if (nearestNode != null && nearestNode.canTotemMove)
                {
                    currentNode = nearestNode;
                    currentNode.isStacked = true;
                    //_transform.DOMove(currentNode.transform.position, 1f);
                    _transform.position = nearestNode.transform.position;
                }
            }
            return _transform;
            
        }

    }

    public class TotemController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Range(0.1f, 2f)]
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
            isControlEnabled = true;
        }

        private void Start()
        {
            totemSettings.SnapToNearestNode(transform, false);
            canMoveNodes = totemSettings.FindPathStart(startNode);
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

                List<Node> testNodes = totemSettings.pathfinder.FindPath(totemSettings.currentNode, totemSettings.nextNode);

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

            StopAllCoroutines();

            transform.DOMove(totemSettings.SnapToNearestNode(transform, false).position, 0.5f);
            //���߿� ������ �ƴ����� �� ���ɼ��� ������ >>> �ƴѰͰ��⵵   
        }
        public IEnumerator FollowPathRoutine(List<Node> path)
        {
            if (path == null || path.Count <= 1)
            {
                Debug.Log("PLAYERCONTROLLER FollowPathRoutine: invalid path");
            }
            else
            {
                for (int i = 0; i < path.Count; i++)
                {
                    totemSettings.nextNode = path[i];
                    yield return StartCoroutine(MoveToNodeRoutine(transform.position, totemSettings.nextNode));
                }
            }
        }

        //  lerp to another Node from current position
        private IEnumerator MoveToNodeRoutine(Vector3 startPosition, Node targetNode)
        {

            float elapsedTime = 0;

            Boundary dirBoundary = totemSettings.currentNode.FindEdge(targetNode);

            if (dirBoundary == null)
            {
                yield break;
            }

            // validate move time
            
            while (elapsedTime < moveTime && targetNode != null && !HasReachedBoundary(dirBoundary))
            {

                elapsedTime += Time.deltaTime;
                float lerpValue = Mathf.Clamp(elapsedTime / moveTime, 0f, 1f);
                Vector3 targetPos = targetNode.transform.position;
                transform.position = Vector3.Lerp(startPosition, targetPos, lerpValue);

                
                // wait one frame
                yield return null;
            }

            if (dirBoundary.isTeleport)
            {
                Boundary revDirBoundary = targetNode.FindEdge(totemSettings.currentNode);


                transform.position = revDirBoundary.transform.position;
            }



            transform.parent = targetNode.transform;
            totemSettings.currentNode.isStacked = false;
            totemSettings.currentNode = targetNode;
            totemSettings.currentNode.isStacked = true;

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

        public bool HasReachedBoundary(Boundary boundary)
        {
            if (totemSettings.pathfinder == null || totemSettings.graph == null || boundary == null)
            {
                return false;
            }

            float distanceSqr = (boundary.transform.position - transform.position).sqrMagnitude;

            return (distanceSqr < 0.01f);
        }


    }
}
