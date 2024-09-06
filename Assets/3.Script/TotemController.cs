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
        [Header("토템세팅")]
        public Transform target;
        public float moveSpeed = 1f;
        public Transform pivot;
        
        
        public int minDragDist = 10;
        public UnityEvent snapEvent; //이건 사용할 지는 모르겠네
        

        
        [HideInInspector] public float previousAngleToMouse;
        [HideInInspector] public bool isActive = true;


        //이 부분은 부모 객체에서 수행해야 할 부분
        private Camera mainCamera;
       
        [SerializeField] private Pathfinder pathfinder;
        private Graph graph;
        [SerializeField] private List<Node> Nodes;
        public Node currentNode;

        public Node nextNode;


        public float moveTime = 1f; // 이동 시간
        
        public float speed;

        //1. 따라서 토템컨트롤러는 부모객체에 달아야 함
        //2. 그에 따라서 pivot들을 체계적으로 관리해야함

        /*해야 될 것
         1. 맨 처음에 토템과 가장 근접한 노드를 찾아 위에 놓기
         2. 추가로 Node에 isTotemMove를 추가하여 isTotemMove 상태에 있는 가장 가까운 노드
         3. 마우스를 드래그해서 드래그한 위치에 있는 가장 가까운 노드를 찾은 후
         4. 플레이어 컨트롤에 있는  FindBestPath 등을 활용해서 이동시키기
         5. 토템의 특정 노드가 이동 가능한 거리로 닿아 있을 때 활성화할 수 있도록 Event
         6. 토템에 대한 추가적인 로직 세팅은.....
         7. 진짜 조영준은 개쓰레기멍청이바보
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
            //pivot들 중 가장 가까운 pivot을 찾기?
            
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
            //쓰레기메서드
            Nodes = pathfinder.FindPath(currentNode, destNode);



            target.position += directionToMouse * moveSpeed * Time.deltaTime;

            currentNode = destNode;
            SnapToNearestNode(target, false);
            //Debug.Log("destNode : " + destNode.transform.name);
            //Move(pathfinder.FindBestPath(currentNode, Nodes));


            //단순히 이동하는 것이 아닌, directionToMouse와 가장 가까운 Node를 찾기
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
            // 텔레포트 노드일 경우 즉시 위치 설정
            if (currentNode.isTeleport)
            {
                transform.position = targetNode.transform.position;
                transform.parent = targetNode.transform;
                currentNode = targetNode;

                targetNode.gameEvent.Invoke(); // 이벤트 호출
                return;
            }

            // 이동 거리 계산
            float distance = Vector3.Distance(startPosition, targetNode.transform.position);

            // 이동 시간을 0.1f ~ 5f 사이로 제한
            moveTime = Mathf.Clamp(moveTime, 0.1f, 5f);
            speed = distance / moveTime;

            transform.Translate(targetNode.transform.position);

       
            transform.parent = targetNode.transform;
            currentNode = targetNode;
            targetNode.gameEvent.Invoke();

        }

        //이부분을 단순히 스냅하는게 아닌 해당하는 마우스 위치에 있는 가장 가까운 노드를 반환시켜 현재 노드부터 그 노드까지 패스파인딩 하면 끝


        //토템 이동 이벤트에서 사용할 것 >>> 이 부분은 아이다도 사용할 가능성이 높다.
        //TODO: PlayerController 스크립트에서도 추가하기, 이 스크립트들에 대한 설계를 다시 해야할 수도

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

       
        [Header("토템 세팅")]
        [SerializeField] private TotemSettings totemSettings;

        [Header("컨트롤 제어권")]
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
            //드래그를 통해 해당하는 노드들만 이동하기 >> 노드가 있는 위치가 아니어도 가장 가깝고 이동 가능한 노드를 찾아서 그 방향으로 토템 이동하는 알고리즘
            if (totemSettings.isActive)
            {
                totemSettings.Drag(eventData.position);
                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //나중에 쓸지도 아닐지도 쓸 가능성이 높을듯 >>> 아닌것같기도
        }



    }
}


