using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

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
        

        
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;


        //�� �κ��� �θ� ��ü���� �����ؾ� �� �κ�
        private Camera mainCamera;
       
        [SerializeField] private Pathfinder pathfinder;
        private Graph graph;
        [SerializeField] private List<Node> Nodes;
        public Node currentNode;

        public Node nextNode;


        public float moveTime = 1f; // �̵� �ð�
        
        public float speed;

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

        public void Initialize()
        {
            mainCamera = Camera.main;

            if (pathfinder != null)
            {
                graph = pathfinder.GetComponent<Graph>();
            }
            
        }

        public void BeginDrag(Vector2 mousePosition)
        {
            //isSpinning = true;
            //pivot�� �� ���� ����� pivot�� ã��?
            
            Vector3 directionToMouse = mousePosition - (Vector2)Camera.main.WorldToScreenPoint(pivot.position);
            

            previousAngleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            target.position = directionToMouse;
            SnapToNearestNode(target, false);
        }

        public void Drag(Vector2 mousePosition)
        {
            if (isActive)
            {
                MoveTotem(mousePosition);
            }
            
        }

        private void MoveTotem(Vector2 mousePosition)
        {
            
            Vector3 directionToMouse = mousePosition - (Vector2)mainCamera.WorldToScreenPoint(pivot.position);

            Node destNode = graph?.FindClosestNode(directionToMouse, true);
            //������޼���
            Nodes = pathfinder.FindPath(currentNode, destNode);



            target.position += directionToMouse * moveSpeed * Time.deltaTime;

            currentNode = destNode;
            SnapToNearestNode(target, false);
            //Debug.Log("destNode : " + destNode.transform.name);
            //Move(pathfinder.FindBestPath(currentNode, Nodes));


            //�ܼ��� �̵��ϴ� ���� �ƴ�, directionToMouse�� ���� ����� Node�� ã��
        }

        

        //private void Move(List<Node> nodes)
        //{
        //    for(int i = 0; i < nodes.Count; i++)
        //    {
        //        nodes[i]
        //    }
        //
        //    
        //}



        public void SnapToNearestNode(Transform _transform, bool isSmoothMove)
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

            
        }

        public void FollowPathRoutine(Transform tr ,List<Node> path)
        {

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
                    nextNode = path[i];

                    // aim at the Node after that to minimize flipping
                    MoveToNode(tr, currentNode.transform.position, nextNode);
                }
            }

        }

        public void MoveToNode(Transform transform, Vector3 startPosition, Node targetNode)
        {
            // �ڷ���Ʈ ����� ��� ��� ��ġ ����
            if (currentNode.isTeleport)
            {
                transform.position = targetNode.transform.position;
                transform.parent = targetNode.transform;
                currentNode = targetNode;

                targetNode.gameEvent.Invoke(); // �̺�Ʈ ȣ��
                return;
            }

            // �̵� �Ÿ� ���
            float distance = Vector3.Distance(startPosition, targetNode.transform.position);

            // �̵� �ð��� 0.1f ~ 5f ���̷� ����
            moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);
            speed = distance / moveTime;

            transform.Translate(targetNode.transform.position);

       
            transform.parent = targetNode.transform;
            currentNode = targetNode;
            targetNode.gameEvent.Invoke();

        }

        //�̺κ��� �ܼ��� �����ϴ°� �ƴ� �ش��ϴ� ���콺 ��ġ�� �ִ� ���� ����� ��带 ��ȯ���� ���� ������ �� ������ �н����ε� �ϸ� ��


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

        [Header("��Ʈ�� �����")]
        public bool isControlEnabled = false;


        private void Awake()
        {
            totemSettings.Initialize();
            //isMoving = false;
            isControlEnabled = true;
        }

        private void Start()
        {

            totemSettings.SnapToNearestNode(transform, false);

            //Vector3.Slerp()
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
                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //���߿� ������ �ƴ����� �� ���ɼ��� ������ >>> �ƴѰͰ��⵵
        }



    }
}


